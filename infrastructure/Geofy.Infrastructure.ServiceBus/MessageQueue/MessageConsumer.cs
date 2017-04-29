using System;
using System.Threading.Tasks;

namespace Geofy.Infrastructure.ServiceBus.MessageQueue
{
    public class MessageConsumer<T> : IMessageConsumer<T> where T : class
    {
        private readonly IMessageQueue<T> _messageQueue;

        private bool _isStarted = false;

        public MessageConsumer(IMessageQueue<T> messageQueue)
        {
            _messageQueue = messageQueue;
        }


        public void Run(Func<object, Task> handler)
        {
            if (_isStarted) throw new InvalidOperationException("Consumer already started");
            _messageQueue.RegisterConsumers(handler);
            _isStarted = true;
        }
    }
}