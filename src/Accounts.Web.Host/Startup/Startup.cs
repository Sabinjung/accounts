﻿using System;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Cors.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Castle.Facilities.Logging;
using Swashbuckle.AspNetCore.Swagger;
using Abp.AspNetCore;
using Abp.Castle.Logging.Log4Net;
using Abp.Extensions;
using Accounts.Configuration;
using Accounts.Identity;

using Abp.AspNetCore.SignalR.Hubs;
using Microsoft.AspNetCore.Mvc;
using Accounts.Blob;
using Accounts.Web.Host.Controllers;
using Intuit.Ipp.OAuth2PlatformClient;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using Abp.Authorization.Users;
using Accounts.Authorization.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Accounts.Core.Invoicing.Intuit;

namespace Accounts.Web.Host.Startup
{
    public class Startup
    {
        private const string _defaultCorsPolicyName = "localhost";

        private readonly IConfigurationRoot _appConfiguration;

        public Startup(IHostingEnvironment env)
        {
            _appConfiguration = env.GetAppConfiguration();
        }

        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.Configure<AzureBlobSettings>(_appConfiguration.GetSection("AzureBlob"));
            services.Configure<IntuitSettings>(_appConfiguration.GetSection("Intuit"));

            services.AddAuthentication()
                 .AddOpenIdConnect("Intuit", o =>
                  {

                      o.ClientId = _appConfiguration["Intuit:ClientId"];
                      o.ClientSecret = _appConfiguration["Intuit:ClientSecret"];
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
                  });
            // MVC
            services.AddMvc(
                options => options.Filters.Add(new CorsAuthorizationFilterFactory(_defaultCorsPolicyName))
            ).SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            IdentityRegistrar.Register(services);
            AuthConfigurer.Configure(services, _appConfiguration);

            services.AddSignalR();

            // Configure CORS for angular2 UI
            services.AddCors(
                options => options.AddPolicy(
                    _defaultCorsPolicyName,
                    builder => builder
                        .WithOrigins(
                            // App:CorsOrigins in appsettings.json can contain more than one address separated by comma.
                            _appConfiguration["App:CorsOrigins"]
                                .Split(",", StringSplitOptions.RemoveEmptyEntries)
                                .Select(o => o.RemovePostFix("/"))
                                .ToArray()
                        )
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials()
                )
            );

            // Swagger - Enable this line and the related lines in Configure method to enable swagger UI
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new Info { Title = "Accounts API", Version = "v1" });
                options.DocInclusionPredicate((docName, description) => true);

                // Define the BearerAuth scheme that's in use
                options.AddSecurityDefinition("bearerAuth", new ApiKeyScheme()
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    In = "header",
                    Type = "apiKey"
                });
            });

            // Configure Abp and Dependency Injection
            return services.AddAbp<AccountsWebHostModule>(
                // Configure Log4Net logging
                options => options.IocManager.IocContainer.AddFacility<LoggingFacility>(
                    f => f.UseAbpLog4Net().WithConfig("log4net.config")
                )
            );
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            app.UseAbp(options => { options.UseAbpRequestLocalization = false; options.UseSecurityHeaders = false; }); // Initializes ABP framework.

            app.UseCors(_defaultCorsPolicyName); // Enable CORS!

            app.UseStaticFiles();

            app.UseAuthentication();

            app.UseAbpRequestLocalization();

            app.UseSignalR(routes =>
            {
                routes.MapHub<AbpCommonHub>("/signalr");
            });

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "defaultWithArea",
                    template: "{area}/{controller=Home}/{action=Index}/{id?}");

                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            // Enable middleware to serve generated Swagger as a JSON endpoint
            app.UseSwagger();
            // Enable middleware to serve swagger-ui assets (HTML, JS, CSS etc.)
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint(_appConfiguration["App:ServerRootAddress"].EnsureEndsWith('/') + "swagger/v1/swagger.json", "Accounts API V1");
                options.IndexStream = () => Assembly.GetExecutingAssembly()
                    .GetManifestResourceStream("Accounts.Web.Host.wwwroot.swagger.ui.index.html");
            }); // URL: /swagger
        }
    }
}
