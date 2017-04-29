using System;
using System.Threading.Tasks;
using Geofy.Infrastructure.ServiceBus.Dispatching.Interfaces;
using Geofy.Infrastructure.ServiceBus.Interfaces;

namespace Geofy.Infrastructure.ServiceBus.RabbitMq
{
    public class EventReplayMessageBus : IMessageBus
    {
        private readonly IDispatcher _dispatcher;

        public EventReplayMessageBus(IDispatcher dispatcher)
        {
            _dispatcher = dispatcher;
        }

        public async Task SendInMemoryAsync(params IMessage[] messages)
        {
            if (messages.Length == 0)
                throw new ArgumentException("messages");

            foreach (var evnt in messages)
                await _dispatcher.DispatchAsync(evnt);
        }

        public Task SendRealTimeMessageAsync(params IMessage[] messages)
        {
            //just don't do anything during reply.
            return Task.CompletedTask;
        }

        public Task SendLongTaskAsync(params IMessage[] messages)
        {
            //just don't do anything during reply.
            return Task.CompletedTask;
        }
    }
}
