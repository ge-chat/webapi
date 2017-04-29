using System;
using System.Threading.Tasks;

namespace Geofy.Infrastructure.ServiceBus.MessageQueue
{
    public interface IMessageConsumer<T> where T : class
    {
        void Run(Func<object, Task> handler);
    }
}