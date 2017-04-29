using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Geofy.Infrastructure.ServiceBus.Interfaces;

namespace Geofy.Infrastructure.ServiceBus.RabbitMq.EventBus
{
    public class MessageQueueEventBus : IEventBus
    {
        private readonly RabbitMqClientsContainer<IEvent> _mqClientsContainer;

        public MessageQueueEventBus(RabbitMqClientsContainer<IEvent> mqClientsContainer)
        {
            _mqClientsContainer = mqClientsContainer;
        }

        public Task PublishAsync(IEvent eventMessage)
        {
            if (eventMessage == null)
                throw new ArgumentNullException(nameof(eventMessage));
            return _mqClientsContainer.GetMesasgeQueueClient(eventMessage).SendAsync(eventMessage);
        }

        public async Task PublishAsync(IEnumerable<IEvent> eventMessages)
        {
            if (eventMessages == null)
                throw new ArgumentNullException(nameof(eventMessages));
            foreach (var evnt in eventMessages)
                await PublishAsync(evnt);
        }
    }
}