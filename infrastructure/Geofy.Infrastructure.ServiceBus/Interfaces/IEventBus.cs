using System.Collections.Generic;
using System.Threading.Tasks;

namespace Geofy.Infrastructure.ServiceBus.Interfaces
{
    public interface IEventBus
    {
        Task PublishAsync(IEvent eventMessage);
        Task PublishAsync(IEnumerable<IEvent> eventMessages);
    }
}