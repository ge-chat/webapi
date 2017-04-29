using System;
using System.Threading.Tasks;
using Geofy.Infrastructure.ServiceBus.Dispatching.Interfaces;
using Geofy.Infrastructure.ServiceBus.Interfaces;

namespace Geofy.Infrastructure.ServiceBus.RabbitMq
{
    public class MessageBus : IMessageBus
    {
        private readonly RabbitMqClientsContainer<IMessage> _longTaskContainer;
        private readonly RabbitMqClientsContainer<IMessage> _realTimeContainer;
        private readonly IDispatcher _dispatcher;

        public MessageBus(RabbitMqClientsContainer<IMessage> longTaskContainer, RabbitMqClientsContainer<IMessage> realTimeContainer, IDispatcher dispatcher)
        {
            _longTaskContainer = longTaskContainer;
            _realTimeContainer = realTimeContainer;
            _dispatcher = dispatcher;
        }

        public async Task SendInMemoryAsync(params IMessage[] messages)
        {
            if (messages.Length == 0)
                throw new ArgumentException("messages");

            foreach (var evnt in messages)
                await _dispatcher.DispatchAsync(evnt);
        }

        public async Task SendRealTimeMessageAsync(params IMessage[] messages)
        {
            if (messages.Length == 0)
                throw new ArgumentException("messages");

            foreach (var message in messages)
                await _realTimeContainer.GetMesasgeQueueClient(message).SendAsync(message);
        }

        public async Task SendLongTaskAsync(params IMessage[] messages)
        {
            if (messages.Length == 0)
                throw new ArgumentException("messages");

            foreach (var message in messages)
                await _longTaskContainer.GetMesasgeQueueClient(message).SendAsync(message);
        }
    }
}
