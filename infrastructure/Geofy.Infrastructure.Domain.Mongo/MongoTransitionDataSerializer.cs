using System;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;

namespace Geofy.Infrastructure.Domain.Mongo
{
    public class MongoTransitionDataSerializer
    {
        public Object Deserialize(BsonDocument doc, Type type)
        {
            return BsonSerializer.Deserialize(doc, type);
        }

        public BsonDocument Serialize(Object obj)
        {
            var data = new BsonDocument();

            var writer = new BsonDocumentWriter(data);
            BsonSerializer.Serialize(writer, obj.GetType(), obj);

            return data;
        }
    }
}
