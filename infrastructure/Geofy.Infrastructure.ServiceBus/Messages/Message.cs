using Geofy.Infrastructure.ServiceBus.Interfaces;

namespace Geofy.Infrastructure.ServiceBus.Messages
{
    public class Message : IMessage
    {
        public MessageMetadata Metadata { get; set; }
    }
}
