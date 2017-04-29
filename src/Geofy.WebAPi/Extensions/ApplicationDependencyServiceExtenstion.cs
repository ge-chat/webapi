using Geofy.Infrastructure.ServiceBus.Dispatching.Interfaces;
using Geofy.Infrastructure.ServiceBus.Interfaces;
using Geofy.Infrastructure.ServiceBus.RabbitMq;
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
            var container = new ApplicationDependencyService()
                .Configure(new Container(), configuration);

            var dispatcher = container.GetInstance<IDispatcher>();
            var rabbitMqClient = container.GetInstance<RabbitMqClientsContainer<ICommand>>();
            var longTasksRabbitMqClient = container.GetInstance<RabbitMqClientsContainer<IMessage>>();
            var eventsRabbitMqClient = container.GetInstance<RabbitMqClientsContainer<IEvent>>();

            var commandsConsumers = rabbitMqClient.GetAllClients();
            var eventConsumers = eventsRabbitMqClient.GetAllClients();
            var longTasksConsumers = longTasksRabbitMqClient.GetAllClients();

            foreach (var commanConsumer in commandsConsumers)
            {
                commanConsumer.RegisterConsumers(x => dispatcher.DispatchAsync(x));
            }

            foreach (var longTaskConsumer in longTasksConsumers)
            {
                longTaskConsumer.RegisterConsumers(x => dispatcher.DispatchAsync(x));
            }

            foreach (var eventConsumer in eventConsumers)
            {
                eventConsumer.RegisterConsumers(x => dispatcher.DispatchAsync(x));
            }

            return container;
        }
    }
}