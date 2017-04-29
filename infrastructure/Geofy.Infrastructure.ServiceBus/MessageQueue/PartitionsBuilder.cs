using System;
using System.Collections.Generic;
using CryptographicHelper = Geofy.Infrastructure.ServiceBus.Helpers.CryptographicHelper;

namespace Geofy.Infrastructure.ServiceBus.MessageQueue
{
    public class PartitionsBuilder<T> where T : class
    {
        private readonly CryptographicHelper _crypto;
        private readonly int _partionsCount;
        private readonly QueueMessageRoutesCollection _routesCollection;
        private readonly Dictionary<long, string> _partionQueueName = new Dictionary<long, string>();

        public PartitionsBuilder(CryptographicHelper crypto, string queueName, int partionsCount, QueueMessageRoutesCollection routesCollection)
        {
            _crypto = crypto;
            _partionsCount = partionsCount;
            _routesCollection = routesCollection;

            _partionQueueName = new Dictionary<long, string>();
            for (long i = 0; i < partionsCount; i++)
            {
                _partionQueueName.Add(i, string.Format("{0}_partition_{1}", queueName, i));
            }
        }

        public long GetPartionNumber(T message)
        {
            var routeKey = _routesCollection.GetRouteKey(message);
            if (routeKey == null)
                return 0;

            return Math.Abs(_crypto.GetHash(routeKey) % _partionsCount);
        }

        /// <summary>
        /// Partion key / Queue name
        /// </summary>
        public Dictionary<long, string> Partitions
        {
            get
            {
                return _partionQueueName;
            }
        }
    }
}
