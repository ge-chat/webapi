﻿using System.Collections.Generic;
using Geofy.ReadModels;
using MongoDB.Driver.GeoJsonObjectModel;

namespace Geofy.WebAPi.ViewModels.Chart
{
    public class ChartViewModel
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public Location Location { get; set; }
        public double Radius { get; set; }
        public string OwnerId { get; set; }
        public IList<string> AdminIds { get; set; } = new List<string>();
        public IList<Participant> Participants { get; set; } = new List<Participant>();
        public IList<MessageReadModel> Messages { get; set; } = new List<MessageReadModel>();
    }
}