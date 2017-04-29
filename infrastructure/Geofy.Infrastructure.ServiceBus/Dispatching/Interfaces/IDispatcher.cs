using System;
using System.Threading.Tasks;

namespace Geofy.Infrastructure.ServiceBus.Dispatching.Interfaces
{
    public interface IDispatcher
    {
        Task DispatchAsync(Object message);
        Task DispatchAsync(Object message, Action<Exception> exceptionObserver);
    }
}