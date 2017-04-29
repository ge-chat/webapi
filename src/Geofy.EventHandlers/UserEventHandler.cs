using System.Threading.Tasks;
using Geofy.Domain.Events.User;
using Geofy.Infrastructure.ServiceBus.Dispatching.Interfaces;
using Geofy.ReadModels;
using Geofy.ReadModels.Services.Databases;
using MongoDB.Driver;

namespace Geofy.EventHandlers
{
    public class UserEventHandler 
        : IMessageHandlerAsync<UserCreated>
    {
        private readonly IMongoCollection<UserReadModel> _userMongoCollection; 

        public UserEventHandler(MongoReadModelsDatabase database)
        {
            _userMongoCollection = database.Users;
        }

        public Task HandleAsync(UserCreated message)
        {
            return _userMongoCollection.InsertOneAsync(new UserReadModel
            {
                Email = message.Email,
                PasswordSalt = message.PasswordSalt,
                PasswordHash = message.PasswordHash,
                UserName = message.UserName
            });
        }
    }
}