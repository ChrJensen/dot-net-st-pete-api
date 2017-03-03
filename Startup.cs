using System;
using System.Text;
using dot_net_st_pete_api.Auth;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace dot_net_st_pete_api
{
    public class Startup
    {
        // secret key used to generate jwt - normally stored in environment vars
        private const string SECRET_KEY = "N8P2b9z??jI'y8rNxLA8e3lzGU37dF";
        private readonly SymmetricSecurityKey securityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(SECRET_KEY));

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddOptions();

            // lock down all routes
            services.AddMvc(config =>
            {
                var policy = new AuthorizationPolicyBuilder()
                                 .RequireAuthenticatedUser()
                                 .Build();
                config.Filters.Add(new AuthorizeFilter(policy));
            });

            var jwtAppSettingOptions = Configuration.GetSection(nameof(JwtIssuerOptions));

            // Configure jwt issuer options
            services.Configure<JwtIssuerOptions>(options =>
            {
                options.Issuer = jwtAppSettingOptions[nameof(JwtIssuerOptions.Issuer)];
                options.Audience = jwtAppSettingOptions[nameof(JwtIssuerOptions.Audience)];
                options.SigningCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            // todo: explore middleware approach
            // app.UseSimpleTokenProvider(new JwtIssuer
            // {
            //     Path = "/login",
            //     Audience = "ExampleAudience",
            //     Issuer = "ExampleIssuer",
            //     SigningCredentials = "signingCredentials",
            //     IdentityResolver = GetIdentity
            // });

            app.UseJwtBearerAuthentication(new JwtBearerOptions
            {
                AutomaticAuthenticate = true,
                AutomaticChallenge = true,
                TokenValidationParameters = buildTokenValidationParameters(),
                AuthenticationScheme = JwtBearerDefaults.AuthenticationScheme,
            });

            app.UseMvc();
        }

        private TokenValidationParameters buildTokenValidationParameters()
        {
            var jwtAppSettingOptions = Configuration.GetSection(nameof(JwtIssuerOptions));

            return new TokenValidationParameters
            {
                // validate the keys
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = securityKey,

                // validate the token issuer (iss)
                ValidateIssuer = true,
                ValidIssuer = jwtAppSettingOptions[nameof(JwtIssuerOptions.Issuer)],

                // validate the token audience (aud)
                ValidateAudience = true,
                ValidAudience = jwtAppSettingOptions[nameof(JwtIssuerOptions.Audience)],

                // validate the token is not expired (exp)
                RequireExpirationTime = true,
                ValidateLifetime = true,

                // specify any clock drift allowance - we set to zero
                ClockSkew = TimeSpan.Zero
            };
        }
    }
}
