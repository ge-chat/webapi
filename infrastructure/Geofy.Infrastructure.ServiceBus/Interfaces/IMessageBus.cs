using System.Threading.Tasks;

namespace Geofy.Infrastructure.ServiceBus.Interfaces
{
    /// <summary>
    /// Messages bus for messages between handlers
    /// </summary>
    public interface IMessageBus
    {
        Task SendInMemoryAsync(params IMessage[] messages);
        Task SendRealTimeMessageAsync(params IMessage[] messages);
        Task SendLongTaskAsync(params IMessage[] messages);
    }
}