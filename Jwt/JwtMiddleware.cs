using System;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

// todo: not currently being used - this would replace the JwtController

namespace dot_net_st_pete_api.Jwt
{
    public class JwtMiddleware
    {
        private readonly RequestDelegate next;
        private readonly JwtIssuerOptions jwtIssuerOptions;

        public JwtMiddleware(RequestDelegate next, IOptions<JwtIssuerOptions> jwtIssuerOptions)
        {
            this.next = next;
            this.jwtIssuerOptions = jwtIssuerOptions.Value;
        }

        /**
         * Determines if we should invoke the middleware
         */
        public Task Invoke(HttpContext context)
        {
            // we are only interested in requests to '/login'
            if (!context.Request.Path.Equals(jwtIssuerOptions.Path, StringComparison.Ordinal))
            {
                return next(context);
            }

            // validate request method and content-type
            if (!context.Request.Method.Equals("POST") || !context.Request.ContentType.Equals("application/json"))
            {
                context.Response.StatusCode = 400;
                return context.Response.WriteAsync("Bad request.");
            }

            return GenerateJwt(context);
        }


        /**
         * Validate email / password
         * Generate JWT on successful login
         */
        private async Task GenerateJwt(HttpContext context)
        {
            // var username = context.Request.Form["username"];
            // var password = context.Request.Form["password"];
            var requestBody = GetRequestBody(context);
            var email = "johnrhampton@gmail.com";
            var password = "123";

            var identity = await GetIdentity(email, password);
            if (identity == null)
            {
                context.Response.StatusCode = 400;
                await context.Response.WriteAsync("Invalid username or password.");
                return;
            }

            // add claims to the jwt
            var claims = new Claim[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat,
                  ToUnixEpochDate(jwtIssuerOptions.IssuedAt).ToString(),
                  ClaimValueTypes.Integer64),
            };

            // create jwt, base64 encode
            var jwt = new JwtSecurityToken(
                issuer: jwtIssuerOptions.Issuer,
                audience: jwtIssuerOptions.Audience,
                claims: claims,
                notBefore: jwtIssuerOptions.NotBefore,
                expires: jwtIssuerOptions.Expiration,
                signingCredentials: jwtIssuerOptions.SigningCredentials);

            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            var response = new
            {
                access_token = encodedJwt,
                expires_in = (int)jwtIssuerOptions.ValidFor.TotalSeconds
            };

            // Serialize and return the response
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(JsonConvert.SerializeObject(response, new JsonSerializerSettings { Formatting = Formatting.Indented }));
        }

        /**
         * This allows us to read the request body multiple times using stream rewinds
         */
        private Object GetRequestBody(HttpContext context)
        {
            var requestBody = "";
            var request = context.Request;

            // allows the stream to be used multiple times
            request.EnableRewind();

            // read the request body
            using (StreamReader reader = new StreamReader(request.Body, Encoding.UTF8, true, 1024, true))
            {
                requestBody = reader.ReadToEnd();
            }

            // rewind the request body
            request.Body.Position = 0;

            // return the email / password object
            return new Object();
        }

        private Task<ClaimsIdentity> GetIdentity(string email, string password)
        {
            if (email == "johnrhampton@gmail.com" && password == "123")
            {
                return Task.FromResult(new ClaimsIdentity(new System.Security.Principal.GenericIdentity(email, "Token"), new Claim[] { }));
            }

            // Credentials are invalid, or account doesn't exist
            return Task.FromResult<ClaimsIdentity>(null);
        }

        /// <returns>Date converted to seconds since Unix epoch (Jan 1, 1970, midnight UTC).</returns>
        private static long ToUnixEpochDate(DateTime date)
          => (long)Math.Round((date.ToUniversalTime() -
                               new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero))
                              .TotalSeconds);
    }
}
