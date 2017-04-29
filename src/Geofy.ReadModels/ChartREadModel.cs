using System.Collections;
using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver.GeoJsonObjectModel;

namespace Geofy.ReadModels
{
    public class ChartReadModel
    {
        [BsonId]
        public string Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public GeoJsonPoint<GeoJson2DGeographicCoordinates> Location { get; set; }
        public double Radius { get; set; }
        public string OwnerId { get; set; }
        public IList<string> AdminIds { get; set; } = new List<string>();
        public IList<Participant> Participants { get; set; } = new List<Participant>();
        public ShortMessage LastMessage { get; set; }
    }
}