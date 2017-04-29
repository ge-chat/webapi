using System;
using AspNet.Security.OpenIdConnect.Server;
using Geofy.WebAPi.Authorization;
using Geofy.WebAPi.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Geofy.WebAPi
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json")
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json");

            builder.AddEnvironmentVariables();
            _configuration = builder.Build();
        }

        private readonly IConfigurationRoot _configuration;

        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddMvc()
                .AddJsonOptions(
                    options =>
                        options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver());
            services.AddAuthentication();
            services.AddSignalR();
            return services.BuildContainer(_configuration);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {

            loggerFactory.AddConsole(_configuration.GetSection("Logging"))
                .AddDebug();

            app.UseSignalRJwtAuthentication()
                .UseJwtBearerAuthentication(new JwtBearerOptions
                {
                    AutomaticAuthenticate = true,
                    AutomaticChallenge = true,
                    RequireHttpsMetadata = false,

                    Audience = "http://localhost:5000/",    
                    Authority = "http://localhost:5000/"
                })
                .UseOpenIdConnectServer(new OpenIdConnectServerOptions
                {
                    Provider = new AuthorizationServerProvider(),
                    AllowInsecureHttp = true,
                    TokenEndpointPath = "/token",
                    Issuer = new Uri("http://localhost:5000/"),

                    //temp solution
                    AccessTokenLifetime = TimeSpan.FromHours(24),
                    RefreshTokenLifetime = TimeSpan.FromHours(24),
                }.UseJwtTokens())
                .UseWebSockets()
                .UseSignalR()
                .UseMvc();
        }
    }
}
