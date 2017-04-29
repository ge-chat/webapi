using System.Collections.Generic;
using System.Linq;
using Geofy.Infrastructure.ServiceBus.Logging;
using Geofy.Infrastructure.ServiceBus.MessageQueue;

namespace Geofy.Infrastructure.ServiceBus.RabbitMq
{
    public class RabbitMqClientsContainer<T> where T : class
    {
        private readonly PartitionsBuilder<T> _partitionBuilder;
        private readonly Dictionary<long, IMessageQueue<T>> _queues;

        public RabbitMqClientsContainer(PartitionsBuilder<T> partitionBuilder, RabbitConnectionSettings settings, ILogFactory loggingFactory)
        {
            _partitionBuilder = partitionBuilder;
            _queues = new Dictionary<long, IMessageQueue<T>>();

            foreach (var partition in partitionBuilder.Partitions)
                _queues.Add(partition.Key, new RabbitMqClient<T>(settings, loggingFactory, partition.Value));
        }

        public IMessageQueue<T> GetMesasgeQueueClient(T message)
        {
            var partitionKey = _partitionBuilder.GetPartionNumber(message);

            return _queues[partitionKey];
        }

        public List<IMessageQueue<T>> GetAllClients()
        {
            return _queues.Values.ToList();
        }
    }
}
