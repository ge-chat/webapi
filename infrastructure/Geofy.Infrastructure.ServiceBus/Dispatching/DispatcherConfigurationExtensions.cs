using System;
using System.Reflection;

namespace Geofy.Infrastructure.ServiceBus.Dispatching
{
    public static class DispatcherConfigurationExtensions
    {
        public static DispatcherConfiguration SetServiceProvider(this DispatcherConfiguration configuration, IServiceProvider container)
        {
            configuration.ServiceProvider = container;
            return configuration;
        }

        public static DispatcherConfiguration SetMaxRetries(this DispatcherConfiguration configuration, Int32 maxRetries)
        {
            configuration.NumberOfRetries = maxRetries;
            return configuration;
        }

        public static DispatcherConfiguration AddHandlers(this DispatcherConfiguration configuration, Assembly assembly, String[] namespaces, NamespaceComparison namespaceComparison = NamespaceComparison.Include)
        {
            configuration.DispatcherHandlerRegistry.Register(assembly, namespaces, namespaceComparison);
            return configuration;
        }

        public static DispatcherConfiguration AddInterceptor(this DispatcherConfiguration configuration, Type interceptor)
        {
            configuration.DispatcherHandlerRegistry.AddInterceptor(interceptor);
            return configuration;
        }

        public static DispatcherConfiguration AddHandlers(this DispatcherConfiguration configuration, Assembly assembly)
        {
            return AddHandlers(configuration, assembly, new string[] { });
        }
    }
}