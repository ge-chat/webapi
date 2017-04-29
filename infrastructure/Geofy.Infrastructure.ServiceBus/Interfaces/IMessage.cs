namespace Geofy.Infrastructure.ServiceBus.Interfaces
{
    public interface IMessage
    {
        MessageMetadata Metadata { get; set; }
    }

    public class MessageMetadata
    {
        public string MessageId { get; set; }

        public string UserId { get; set; }
    }
}
