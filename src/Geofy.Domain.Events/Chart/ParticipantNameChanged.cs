using Geofy.Infrastructure.ServiceBus.Messages;

namespace Geofy.Domain.Events.Chart
{
    public class ParticipantNameChanged : Event
    {
        public string ChatId { get; set; }
        public string Name { get; set; }
        public string UserId { get; set; }
    }
}