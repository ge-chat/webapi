using System.Collections.Generic;
using System.Threading.Tasks;
using Geofy.Infrastructure.ServiceBus.Dispatching.Interfaces;
using Geofy.Infrastructure.ServiceBus.Interfaces;

namespace Geofy.Infrastructure.ServiceBus.RabbitMq.EventBus
{
    public class DispatcherEventBus : IEventBus
    {
        private readonly IDispatcher _dispatcher;

        public DispatcherEventBus(IDispatcher dispatcher)
        {
            _dispatcher = dispatcher;
        }

        public Task PublishAsync(IEvent eventMessage)
        {
            return _dispatcher.DispatchAsync(eventMessage);
        }

        public async Task PublishAsync(IEnumerable<IEvent> eventMessages)
        {
            foreach (var evnt in eventMessages)
                await _dispatcher.DispatchAsync(evnt);
        }
    }
}
