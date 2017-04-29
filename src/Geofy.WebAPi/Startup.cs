using EasyNetQ;
using Geofy.WebAPi.Extensions;
using Geofy.WebAPI.DependencyInjection;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using StructureMap;
using IServiceProvider = System.IServiceProvider;

namespace Geofy.WebAPi
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json");

            if (env.IsEnvironment("Development"))
            {
                builder.AddApplicationInsightsSettings(developerMode: true)
                    .AddJsonFile($"appsettings.{env.EnvironmentName}.json");

            }

            builder.AddEnvironmentVariables();
            _configuration = builder.Build().ReloadOnChanged("appsettings.json");
        }

        private readonly IConfigurationRoot _configuration;

        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddApplicationInsightsTelemetry(_configuration);
            services.AddMvc();
            var container = services.AddApplicationDependencies(_configuration);
            container.Populate(services);

            return new StructureMapServiceProvider(container);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {

            loggerFactory.AddConsole(_configuration.GetSection("Logging"))
                .AddDebug();

            app.UseApplicationInsightsRequestTelemetry()
                .UseApplicationInsightsExceptionTelemetry();

            app.UseMvc();
        }

        // Entry point for the application.
        public static void Main(string[] args) => WebApplication.Run<Startup>(args);
    }
}
