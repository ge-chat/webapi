using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;

namespace Geofy.Shared.Mongo
{
    public class NoDefaultPropertyIdConvention : IClassMapConvention
    {
        public string Name => "No Default Property Id Convention";

        public void Apply(BsonClassMap classMap)
        {
            classMap.SetIdMember(null);
        }
    }
}