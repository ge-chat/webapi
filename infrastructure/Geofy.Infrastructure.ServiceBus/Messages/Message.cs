using Geofy.Infrastructure.ServiceBus.Interfaces;

namespace Geofy.Infrastructure.ServiceBus.Messages
{
    public class Message : IMessage
    {
        public string Id { get; set; }

        public string TenantId { get; set; }

        public MessageMetadata Metadata { get; set; }
    }
}
