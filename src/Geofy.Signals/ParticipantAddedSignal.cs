using Geofy.Infrastructure.ServiceBus.Messages;
using Geofy.ReadModels;

namespace Geofy.Signals
{
    public class ParticipantAddedSignal : Message
    {
        public Participant Participant { get; set; }
        public string ChartId { get; set; }
    }
}