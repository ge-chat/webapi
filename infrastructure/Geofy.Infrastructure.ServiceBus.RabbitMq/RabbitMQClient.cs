using System;
using System.Threading.Tasks;
using EasyNetQ;
using Geofy.Infrastructure.ServiceBus.Logging;
using Geofy.Infrastructure.ServiceBus.MessageQueue;

namespace Geofy.Infrastructure.ServiceBus.RabbitMq
{
    public class RabbitMqClient<T> : IMessageQueue<T>, IDisposable where T : class
    {
        private readonly string _queueName;
        private readonly IBus _bus;
        private IDisposable _consumer;

        public RabbitMqClient(RabbitConnectionSettings settings, ILogFactory loggingFactory, string queueName)
        {
            _queueName = queueName;
            var logger = new MessageQueueLogger(loggingFactory);
            _bus = RabbitHutch.CreateBus(GetConnectionString(settings), x => x.Register<IEasyNetQLogger>(_ => logger));
        }

        public Task SendAsync(T message)
        {
            return _bus.SendAsync(_queueName, message);
        }

        public void RegisterConsumers(Func<T, Task> handler)
        {
            _consumer = _bus.Receive(_queueName, handler);
        }

        public void Dispose()
        {
            _consumer?.Dispose();

            _bus.Dispose();
        }

        private string GetConnectionString(RabbitConnectionSettings settings)
        {
            return
                $"host={settings.RabbitHostName};requestedHeartbeat=10;username={settings.RabbitUser};password={settings.RabbitPass};publisherConfirms=true";
        }
    }
}