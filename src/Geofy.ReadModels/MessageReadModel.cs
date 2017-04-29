using System;
using MongoDB.Bson.Serialization.Attributes;

namespace Geofy.ReadModels
{
    public class MessageReadModel
    {
        [BsonId]
        public string Id { get; set; }
        public string UserId { get; set; }
        public DateTime Created { get; set; }
        public string Message { get; set; }
    }
}