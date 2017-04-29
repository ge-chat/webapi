using Geofy.Infrastructure.ServiceBus.Messages;

namespace Geofy.Signals
{
    public class ParticipantNameChangedSignal : Message
    {
        public string ChatId { get; set; }
        public string Name { get; set; }
        public string UserId { get; set; }
    }
}