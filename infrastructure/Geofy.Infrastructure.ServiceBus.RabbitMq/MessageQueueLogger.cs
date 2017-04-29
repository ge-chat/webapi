using System;
using EasyNetQ;
using Geofy.Infrastructure.ServiceBus.Logging;

namespace Geofy.Infrastructure.ServiceBus.RabbitMq
{
    public class MessageQueueLogger : IEasyNetQLogger
    {
        private readonly ILoggingService _log;

        public MessageQueueLogger(ILogFactory loggingFactory)
        {
            _log = loggingFactory.GetLoggingService(ToString());
        }


        public void DebugWrite(string format, params object[] args)
        {
            _log.Trace(format, args);
        }

        public void ErrorWrite(Exception exception)
        {
            _log.Error(exception);
        }

        public void ErrorWrite(string format, params object[] args)
        {
            _log.Error(format, args);
        }

        public void InfoWrite(string format, params object[] args)
        {
            _log.Info(format, args);
        }
    }
}
