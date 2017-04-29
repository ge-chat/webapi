using Geofy.Infrastructure.ServiceBus.Messages;

namespace Geofy.Domain.Commands.Chart
{
    public class ChangeParticipantName : Command
    {
        public string ChatId { get; set; }
        public string Name { get; set; }
        public string UserId { get; set; }
    }
}