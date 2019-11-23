using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Abp.Runtime.Security;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Accounts.Authorization;
using Microsoft.AspNetCore.Authorization;

namespace Accounts.Web.Host.Startup
{
    public static class AuthConfigurer
    {
        public static void Configure(IServiceCollection services, IConfiguration configuration)
        {
            if (bool.Parse(configuration["Authentication:JwtBearer:IsEnabled"]))
            {
                services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = "JwtBearer";
                    options.DefaultChallengeScheme = "JwtBearer";
                }).AddJwtBearer("JwtBearer", options =>
                {
                    options.Audience = configuration["Authentication:JwtBearer:Audience"];

                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        // The signing key must match!
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(configuration["Authentication:JwtBearer:SecurityKey"])),

                        // Validate the JWT Issuer (iss) claim
                        ValidateIssuer = true,
                        ValidIssuer = configuration["Authentication:JwtBearer:Issuer"],

                        // Validate the JWT Audience (aud) claim
                        ValidateAudience = true,
                        ValidAudience = configuration["Authentication:JwtBearer:Audience"],

                        // Validate the token expiry
                        ValidateLifetime = true,

                        // If you want to allow a certain amount of clock drift, set that here
                        ClockSkew = TimeSpan.Zero
                    };

                    options.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = QueryStringTokenResolver
                    };
                })
                 .AddOpenIdConnect("Intuit", o =>
                 {

                     o.ClientId = configuration["Intuit:ClientId"];
                     o.ClientSecret = configuration["Intuit:ClientSecret"];
                     o.ResponseType = OpenIdConnectResponseType.Code;
                     o.MetadataAddress = "https://developer.api.intuit.com/.well-known/openid_sandbox_configuration/";
                     o.ProtocolValidator.RequireNonce = false;
                     o.SaveTokens = true;
                     o.GetClaimsFromUserInfoEndpoint = true;
                     o.ClaimActions.MapUniqueJsonKey("given_name", "givenName");
                     o.ClaimActions.MapUniqueJsonKey("family_name", "familyName");
                     o.ClaimActions.MapUniqueJsonKey(ClaimTypes.Email, "email"); //should work but because the middleware checks for claims w/ the same value and the claim for "email" already exists it doesn't get mapped.
                     o.Scope.Add("phone");
                     o.Scope.Add("email");
                     o.Scope.Add("address");
                     o.Scope.Add("com.intuit.quickbooks.accounting");
                     o.Events = new OpenIdConnectEvents()
                     {
                         OnAuthenticationFailed = c =>
                         {
                             c.HandleResponse();

                             c.Response.StatusCode = 500;
                             c.Response.ContentType = "text/plain";
                             return c.Response.WriteAsync(c.Exception.ToString());
                         },
                         OnUserInformationReceived = context =>
                         {
                             var identity = (ClaimsIdentity)context.Principal.Identity;
                             var token = context.Properties.GetTokenValue("access_token");
                             identity.AddClaim(new Claim("access_token", token));
                             token = context.Properties.GetTokenValue("refresh_token");
                             identity.AddClaim(new Claim("refresh_token", token));

                             context.Principal.AddIdentity(identity);
                             return Task.CompletedTask;
                         }
                     };

                     o.SaveTokens = true;
                 })
                 .AddGoogle(options =>
                 {
                     IConfigurationSection googleAuthNSection =
                         configuration.GetSection("Authentication:Google");

                     options.ClientId = googleAuthNSection["ClientId"];
                     options.ClientSecret = googleAuthNSection["ClientSecret"];
                 });
            }


            services.AddAuthorization(options =>
            {
                options.AddPolicy("Intuit", policy =>
                {
      
                    policy.AddRequirements(new IntuitRequirement());
                });
            });

            services.AddSingleton<IAuthorizationHandler, IntuitHandler>();
        }

        /* This method is needed to authorize SignalR javascript client.
         * SignalR can not send authorization header. So, we are getting it 
         * from query string as an encrypted text. */
        private static Task QueryStringTokenResolver(Microsoft.AspNetCore.Authentication.JwtBearer.MessageReceivedContext context)
        {
            if (!context.HttpContext.Request.Path.HasValue ||
                !context.HttpContext.Request.Path.Value.StartsWith("/signalr"))
            {
                // We are just looking for signalr clients
                return Task.CompletedTask;
            }

            var qsAuthToken = context.HttpContext.Request.Query["enc_auth_token"].FirstOrDefault();
            if (qsAuthToken == null)
            {
                // Cookie value does not matches to querystring value
                return Task.CompletedTask;
            }

            // Set auth token from cookie
            context.Token = SimpleStringCipher.Instance.Decrypt(qsAuthToken, AppConsts.DefaultPassPhrase);
            return Task.CompletedTask;
        }
    }
}
