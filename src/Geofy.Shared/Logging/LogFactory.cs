using Geofy.Infrastructure.ServiceBus.Logging;  

namespace Geofy.Shared.Logging
{
    public class LogFactory : ILogFactory
    {
        public ILoggingService GetLoggingService(string loggerName)
        {
            var logger = (LoggingService)NLog.LogManager.GetLogger(loggerName, typeof(LoggingService));
            logger._loggerName = loggerName;
            return logger;
        } 
    }
}