using System.Collections.Generic;
using System.Threading.Tasks;
using Geofy.ReadModels.Services.Base;
using Geofy.ReadModels.Services.Databases;
using MongoDB.Driver;

namespace Geofy.ReadModels.Services.User
{
    public class UserReadModelService : BaseReadModelServicece<UserReadModel, UserFilter>
    {
        public UserReadModelService(MongoReadModelsDatabase db) 
            : base(db.Users)
        {
        }

        public override IEnumerable<FilterDefinition<UserReadModel>> BuildFilterQuery(UserFilter filter)
        {
            var builder = Builders<UserReadModel>.Filter;

            if (!string.IsNullOrEmpty(filter.Email))
            {
                yield return builder.Eq(x => x.Email, filter.Email);
            }
        }

        public Task<UserReadModel> GetByEmailAsync(string email)
        {
            var filter = new UserFilter {Email = email};
            return GetCursorByFilter(filter).FirstOrDefaultAsync();
        }
    }
}