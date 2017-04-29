namespace Geofy.Infrastructure.ServiceBus.MessageQueue
{
    public interface IMessageQueueFactory
    {
        IMessageQueue<T> Create<T>(string queueName);
    }
}