namespace Geofy.Infrastructure.ServiceBus.Interfaces
{
    public interface IMessage
    {
        string Id { get; set; }

        string TenantId { get; set; }

        MessageMetadata Metadata { get; set; }
    }

    public class MessageMetadata
    {
        public string MessageId { get; set; }

        public string UserId { get; set; }
    }
}
