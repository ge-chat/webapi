using System;
using Geofy.Domain.User;
using Geofy.EventHandlers;
using Geofy.Infrastructure.Domain.Interfaces;
using Geofy.Infrastructure.Domain.Mongo;
using Geofy.Infrastructure.Domain.Transitions.Interfaces;
using Geofy.Infrastructure.ServiceBus.Dispatching;
using Geofy.Infrastructure.ServiceBus.Dispatching.Interfaces;
using Geofy.Infrastructure.ServiceBus.Helpers;
using Geofy.Infrastructure.ServiceBus.Interfaces;
using Geofy.Infrastructure.ServiceBus.Logging;
using Geofy.Infrastructure.ServiceBus.MessageQueue;
using Geofy.Infrastructure.ServiceBus.Messages;
using Geofy.Infrastructure.ServiceBus.RabbitMq;
using Geofy.Infrastructure.ServiceBus.RabbitMq.EventBus;
using Geofy.ReadModels;
using Geofy.ReadModels.Services.Chart;
using Geofy.ReadModels.Services.Databases;
using Geofy.ReadModels.Services.User;
using Geofy.Services;
using Geofy.Shared.Logging;
using Geofy.Shared.Mongo;
using Geofy.WebAPi.SignalR.Handlers;
using Geofy.WebAPI.DependencyInjection;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using StructureMap;

namespace Geofy.WebAPI.Services
{
    public class ApplicationDependencyService
    {
        public IContainer Configure(IContainer container, IConfiguration configuration)
        {
            ConfigureLogger(container);
            ConfigureSettings(container, configuration);
            ConfigureMongoDb(container);
            ConfigureTransport(container);
            ConfigureEventStore(container);
            ConfigureOther(container);

            return container;
        }

        private void ConfigureSettings(IContainer container, IConfiguration configuration)
        {
            container.Configure(config => config.For<IConfiguration>().Singleton().Use(configuration));
        }

        private void ConfigureLogger(IContainer container)
        {
            container.Configure(config => config.For<ILogFactory>().Use<LogFactory>());
        }

        private void ConfigureMongoDb(IContainer container)
        {
            // Register bson serializer conventions
            var myConventions = new ConventionPack
            {
                new NoDefaultPropertyIdConvention(),
                new IgnoreExtraElementsConvention(true),
            };
            ConventionRegistry.Register("MyConventions", myConventions, type => true);

            BsonSerializer.RegisterSerializer(typeof (DateTime), new DateTimeSerializer(DateTimeKind.Utc));
            BsonSerializer.RegisterSerializer(typeof(DateTime?), new NullableSerializer<DateTime>());
            //driver trying to use connections that have been killed by azure for being idle
            MongoDefaults.MaxConnectionIdleTime = TimeSpan.FromMinutes(1);

            RegisterBsonMaps();

            var configuration = container.GetInstance<IConfiguration>();
            container.Configure(config =>
            {
                config.For<MongoReadModelsDatabase>().Singleton().Use(new MongoReadModelsDatabase(configuration["Data:Mongo:ConnectionString:ReadModels"], false));
                config.For<MongoLogsDatabase>().Singleton().Use(new MongoLogsDatabase(configuration["Data:Mongo:ConnectionString:Logs"], false));
                config.For<MongoEventsDatabase>().Singleton().Use(new MongoEventsDatabase(configuration["Data:Mongo:ConnectionString:Events"], false));
            });
        }

        private static void RegisterBsonMaps()
        {
            //BsonClassMap.RegisterClassMap<UserView>();
        }

        private void ConfigureEventStore(IContainer container)
        {
            var configuration = container.GetInstance<IConfiguration>();
            var transitionsRepository = new MongoTransitionRepository(configuration["Data:Mongo:ConnectionString:Events"], false);

            var eventsMqClientFactory = container.GetInstance<RabbitMqClientsContainer<IEvent>>();
            var eventBus = new MessageQueueEventBus(eventsMqClientFactory);
            container.Configure(config =>
            {
                config.For<ITransitionRepository>().Singleton().Use(transitionsRepository);
                config.For<IRepository>().Use<Repository>();
                config.For(typeof(IRepository<>)).Use(typeof(Repository<>));
                config.For<IEventBus>().Singleton().Use(eventBus);
            });
        }

        private void ConfigureTransport(IContainer container)
        {
            var dispatcher = Dispatcher.Create(config =>
            {
                config.SetServiceProvider(new StructureMapServiceProvider(container));
                config.AddHandlers(typeof(UserAggregate).Assembly, new string[0]);
                config.AddHandlers(typeof(UserEventHandler).Assembly, new string[0]);
                config.AddHandlers(typeof(ChartSignalHandler).Assembly, new string[0]);
                config.SetMaxRetries(3); //retry to handle 3 times before fail
                return config;
            }, container.GetInstance<ILogFactory>());

            var crypto = container.GetInstance<CryptographicHelper>();
            var configuration = container.GetInstance<IConfiguration>();
            var logFactory = container.GetInstance<ILogFactory>();
            var rabbitConfig = new RabbitConnectionSettings
            {
                RabbitHostName = configuration["Transport:RabbitMQ:Host"],
                RabbitUser = configuration["Transport:RabbitMQ:User"],
                RabbitPass = configuration["Transport:RabbitMQ:Password"]
            };


            var routesCollection = MapQueueMessageRoutes(logFactory);

            var commandsPartionBuilder = new PartitionsBuilder<ICommand>(crypto, configuration["Transport:RabbitMQ:CommandQueue:Name"], int.Parse(configuration["Transport:RabbitMQ:CommandQueue:ConsumersCount"]), routesCollection);
            var commandsMqClientFactory = new RabbitMqClientsContainer<ICommand>(commandsPartionBuilder, rabbitConfig, logFactory);

            var eventsPartionBuilder = new PartitionsBuilder<IEvent>(crypto, configuration["Transport:RabbitMQ:EventQueue:Name"], int.Parse(configuration["Transport:RabbitMQ:EventQueue:ConsumersCount"]), routesCollection);
            var eventsMqClientFactory = new RabbitMqClientsContainer<IEvent>(eventsPartionBuilder, rabbitConfig, logFactory);
            var commandBus = new CommandBus(commandsMqClientFactory, logFactory);

            var messagePartionBuilder = new PartitionsBuilder<IMessage>(crypto, configuration["Transport:RabbitMQ:MessageQueue:Name"], int.Parse(configuration["Transport:RabbitMQ:MessageQueue:ConsumersCount"]), routesCollection);
            var messageMqClientFactory = new RabbitMqClientsContainer<IMessage>(messagePartionBuilder, rabbitConfig, logFactory);
            IMessageBus messageBus = new MessageBus(messageMqClientFactory, messageMqClientFactory, dispatcher);

            container.Configure(config =>
            {
                config.For<ICommandBus>().Singleton().Use(commandBus);
                config.For<IDispatcher>().Singleton().Use(dispatcher);
                config.For<PartitionsBuilder<ICommand>>().Singleton().Use(commandsPartionBuilder);
                config.For<RabbitMqClientsContainer<ICommand>>().Singleton().Use(commandsMqClientFactory);
                config.For<PartitionsBuilder<IEvent>>().Singleton().Use(eventsPartionBuilder);
                config.For<RabbitMqClientsContainer<IEvent>>().Singleton().Use(eventsMqClientFactory);
                config.For<IMessageBus>().Singleton().Use(messageBus);
                config.For<RabbitMqClientsContainer<IMessage>>().Singleton().Use(messageMqClientFactory);
            });
        }

        private static void ConfigureOther(IContainer container)
        {
            container.Configure(config =>
            {
                config.For<AuthenticationService>();
                config.For<CryptographicHelper>();
                config.For<IdGenerator>();

                config.For<UserReadModelService>();
                config.For<ChartReadModelService>();
            });
        }

        private static QueueMessageRoutesCollection MapQueueMessageRoutes(ILogFactory logFactory)
        {
            var routesCollection = new QueueMessageRoutesCollection(logFactory);
            //by default everything is partioned by user id
            routesCollection.AddRoute<Command>(command => command.Metadata.UserId);
            routesCollection.AddRoute<Event>(evt => evt.Metadata.UserId);
            routesCollection.AddRoute<Message>(evt => evt.Metadata.UserId);

            return routesCollection;
        }
    }
}
