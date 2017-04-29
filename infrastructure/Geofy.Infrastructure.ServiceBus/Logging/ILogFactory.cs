namespace Geofy.Infrastructure.ServiceBus.Logging
{
    public interface ILogFactory
    {
         ILoggingService GetLoggingService(string loggerName);
    }
}