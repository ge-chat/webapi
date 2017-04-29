namespace Geofy.Infrastructure.ServiceBus.Dispatching.Interfaces
{
    public interface IMessageHandlerInterceptor
    {
        void Intercept(DispatcherInvocationContext context);
    }
}
