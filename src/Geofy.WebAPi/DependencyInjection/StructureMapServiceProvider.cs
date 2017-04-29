using System;
using StructureMap;

namespace Geofy.WebAPI.DependencyInjection
{
    public class StructureMapServiceProvider : IServiceProvider
    {
        private readonly IContainer _container;

        public StructureMapServiceProvider(IContainer container)
        {
            _container = container;
        }

        public object GetService(Type serviceType)
        {
            return _container.GetInstance(serviceType);
        }
    }
}