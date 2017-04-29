using System.Collections.Generic;
using System.Threading.Tasks;

namespace Geofy.Infrastructure.ServiceBus.Interfaces
{
    public class InMemoryEventBus : IEventBus
    {
        public List<IEvent> Events = new List<IEvent>();

        public async Task PublishAsync(IEvent eventMessage)
        {
            Events.Add(eventMessage);
        }

        public async Task PublishAsync(IEnumerable<IEvent> eventMessages)
        {
            Events.AddRange(eventMessages);
        }
    }
}