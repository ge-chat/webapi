using MongoDB.Bson;

namespace Geofy.Shared.Mongo
{
    public class IdGenerator 
    {
        public string Generate()
        {
            return ObjectId.GenerateNewId().ToString();
        }
    }
}
