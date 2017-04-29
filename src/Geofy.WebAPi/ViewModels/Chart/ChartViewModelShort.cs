using System.Collections.Generic;
using Geofy.ReadModels;

namespace Geofy.WebAPi.ViewModels.Chart
{
    public class ChartViewModelShort
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public Location Location { get; set; }
        public double Radius { get; set; }
        public string OwnerId { get; set; }
        public IList<string> AdminIds { get; set; } = new List<string>();
        public IList<Participant> Participants { get; set; } = new List<Participant>();
    }
}