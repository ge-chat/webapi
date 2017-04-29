using System.Collections.Generic;
using System.Threading.Tasks;

namespace Geofy.Infrastructure.ServiceBus.Interfaces
{
    public class InMemoryEventBus : IEventBus
    {
        private readonly List<IEvent> _events = new List<IEvent>();

        public Task PublishAsync(IEvent eventMessage)
        {
            _events.Add(eventMessage);
            return Task.CompletedTask;
        }

        public Task PublishAsync(IEnumerable<IEvent> eventMessages)
        {
            _events.AddRange(eventMessages);
            return Task.CompletedTask;
        }
    }
}