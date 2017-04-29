using MongoDB.Bson.Serialization.Attributes;

namespace Geofy.ReadModels
{
    public class UserReadModel
    {
        [BsonId]
        public string Id { get; set; }
        public string UserName { get; set; }
        public string PasswordHash { get; set; }
        public string PasswordSalt { get; set; }
        public string Email { get; set; }
    }
}