using Geofy.WebAPI.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StructureMap;

namespace Geofy.WebAPi.Extensions
{
    public static class ApplicationDependencyServiceExtenstion
    {
        public static IContainer AddApplicationDependencies(this IServiceCollection collection, IConfiguration configuration)
        {
            var container = new Container();
            return new ApplicationDependencyService().Configure(container, configuration);
        }
    }
}