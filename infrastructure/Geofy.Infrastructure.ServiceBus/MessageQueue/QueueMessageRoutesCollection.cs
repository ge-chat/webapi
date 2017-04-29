using System;
using System.Collections.Generic;
using ILogFactory = Geofy.Infrastructure.ServiceBus.Logging.ILogFactory;
using ILoggingService = Geofy.Infrastructure.ServiceBus.Logging.ILoggingService;

namespace Geofy.Infrastructure.ServiceBus.MessageQueue
{
    public class QueueMessageRoutesCollection
    {
        private ILoggingService _logger;

        public QueueMessageRoutesCollection(ILogFactory logFactory)
        {
            _logger = logFactory.GetLoggingService(GetType().ToString());
        }

        private readonly Dictionary<Type, dynamic> _routes = new Dictionary<Type, dynamic>();

        public void AddRoute<T>(Func<T, string> route) where T : class
        {
            var type = typeof(T);
            _routes.Add(type, route);
        }

        public string GetRouteKey<T>(T message) where T : class
        {
            string routeKey = string.Empty;

            var messageType = message.GetType();
            if (_routes.ContainsKey(messageType))
            {
                routeKey = _routes[messageType]((dynamic)message);
            }
            else
            {
                var baseType = messageType.BaseType;
                while (baseType != null)
                {
                    if (_routes.ContainsKey(baseType))
                    {
                        routeKey = _routes[baseType]((dynamic)message);
                        break;
                    }

                    baseType = baseType.BaseType;
                }
            }

            if (string.IsNullOrEmpty(routeKey))
                _logger.Info("Message queue partion key is not found for message: " + messageType.FullName);

            return routeKey;
        }
    }
}
