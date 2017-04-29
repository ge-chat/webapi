using System.Threading.Tasks;

namespace Geofy.Infrastructure.ServiceBus.Dispatching.Interfaces
{
    public interface IMessageHandlerAsync<in TMessage>
    {
        Task HandleAsync(TMessage message);
    }

    public interface IMessageHandler
    {
        // Marker interface
    }

}
