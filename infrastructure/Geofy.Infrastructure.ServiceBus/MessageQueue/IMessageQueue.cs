using System;
using System.Threading.Tasks;

namespace Geofy.Infrastructure.ServiceBus.MessageQueue
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T">Type of message</typeparam>
    public interface IMessageQueue<T> : IDisposable
    {
        Task SendAsync(T message);
        void RegisterConsumers(Func<T, Task> message);
    }
}