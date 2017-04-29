using Geofy.Infrastructure.ServiceBus.Messages;

namespace Geofy.Domain.Events.Chart
{
    public class ChartCreated : Event
    {
        public string ChartId { get; set; }
        public string Title { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double Radius { get; set; }
        public string OwnerId { get; set; }
        public string Description { get; set; }
    }
}