using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using dot_net_st_pete_api.Models;
using dot_net_st_pete_api.Jwt;
using dot_net_st_pete_api.Repository;

namespace dot_net_st_pete_api.Controllers
{
    [Route("[controller]")]
    public class JwtController : Controller
    {
        private readonly JwtIssuerOptions _jwtOptions;
        private readonly ILogger _logger;
        private readonly JsonSerializerSettings _serializerSettings;
        MongoRepository mongo;

        public JwtController(IOptions<JwtIssuerOptions> jwtOptions, ILoggerFactory loggerFactory, MongoRepository mongo)
        {
            this.mongo = mongo;
            _jwtOptions = jwtOptions.Value;
            ThrowIfInvalidOptions(_jwtOptions);

            _logger = loggerFactory.CreateLogger<JwtController>();

            _serializerSettings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented
            };
        }

        [Route("create")]
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Create([FromBodyAttribute] User user)
        {
            if (user.Email == null || user.Password == null)
            {
                var badUserJson = JsonConvert.SerializeObject(new { message = "Email and Password are required" }, _serializerSettings);
                return new BadRequestObjectResult(badUserJson);
            }

            // hash and save a password
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(user.Password);

            var foundUser = mongo.GetUser(user.Email);
            if (foundUser != null)
            {
                var badUserJson = JsonConvert.SerializeObject(new { message = "Whoops, have you already registered?" }, _serializerSettings);
                return new BadRequestObjectResult(badUserJson);
            }

            var createdUser = mongo.CreateUser(new User
            {
                Email = user.Email,
                Password = hashedPassword
            });

            var json = JsonConvert.SerializeObject(createdUser, _serializerSettings);
            return new OkObjectResult(json);
        }

        [Route("login")]
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBodyAttribute] User user)
        {
            var identity = await VerifyUser(user);
            if (identity == null)
            {
                _logger.LogInformation($"Invalid email ({user.Email}) or password ({user.Password})");
                return BadRequest("Invalid credentials");
            }

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, await _jwtOptions.JtiGenerator()),
                new Claim(JwtRegisteredClaimNames.Iat,
                  ToUnixEpochDate(_jwtOptions.IssuedAt).ToString(),
                  ClaimValueTypes.Integer64),
                  identity.FindFirst("DisneyCharacter")
            };

            // Create the JWT security token and encode it.
            var jwt = new JwtSecurityToken(
                issuer: _jwtOptions.Issuer,
                audience: _jwtOptions.Audience,
                claims: claims,
                notBefore: _jwtOptions.NotBefore,
                expires: _jwtOptions.Expiration,
                signingCredentials: _jwtOptions.SigningCredentials);

            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            // Serialize and return the response
            var response = new
            {
                access_token = encodedJwt,
                expires_in = (int)_jwtOptions.ValidFor.TotalSeconds
            };

            var json = JsonConvert.SerializeObject(response, _serializerSettings);
            return new OkObjectResult(json);
        }

        private static void ThrowIfInvalidOptions(JwtIssuerOptions options)
        {
            if (options == null) throw new ArgumentNullException(nameof(options));

            if (options.ValidFor <= TimeSpan.Zero)
            {
                throw new ArgumentException("Must be a non-zero TimeSpan.", nameof(JwtIssuerOptions.ValidFor));
            }

            if (options.SigningCredentials == null)
            {
                throw new ArgumentNullException(nameof(JwtIssuerOptions.SigningCredentials));
            }

            if (options.JtiGenerator == null)
            {
                throw new ArgumentNullException(nameof(JwtIssuerOptions.JtiGenerator));
            }
        }

        /// <returns>Date converted to seconds since Unix epoch (Jan 1, 1970, midnight UTC).</returns>
        private static long ToUnixEpochDate(DateTime date)
          => (long)Math.Round((date.ToUniversalTime() -
                               new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero))
                              .TotalSeconds);

        /// <summary>
        /// IMAGINE BIG RED WARNING SIGNS HERE!
        /// You'd want to retrieve claims through your claims provider
        /// in whatever way suits you, the below is purely for demo purposes!
        /// </summary>
        private Task<ClaimsIdentity> VerifyUser(User user)
        {
            var foundUser = mongo.GetUser(user.Email);
            if (foundUser == null)
            {
                // user does not exist
                return Task.FromResult<ClaimsIdentity>(null);
            }

            // verify password matches
            bool validPassword = BCrypt.Net.BCrypt.Verify(user.Password, foundUser.Password);
            if (validPassword)
            {
                return Task.FromResult(new ClaimsIdentity(
                                  new GenericIdentity(user.Email, "Token"),
                                  new Claim[] { }));
            }
            else
            {
                // invalid credentials
                return Task.FromResult<ClaimsIdentity>(null);
            }
        }
    }
}