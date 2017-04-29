using System.Threading.Tasks;
using MongoDB.Driver;

namespace Geofy.ReadModels.Services.Databases
{
    public class MongoReadModelsDatabase
    {
        private readonly MongoInstance _mongo;

        public MongoReadModelsDatabase(string connectionString, bool authenticateToAdmin)
        {
            _mongo = new MongoInstance(connectionString, authenticateToAdmin);
            CreateIndexes().Wait();
        }

        /// <summary>
        /// Get database
        /// </summary>
        private IMongoDatabase Database => _mongo.Client.GetDatabase(_mongo.DefaultDatabase);

        private IMongoCollection<TDocument> GetCollection<TDocument>(string collectionName)
        {
            return Database.GetCollection<TDocument>(collectionName);
        }

        public IMongoCollection<UserReadModel> Users => GetCollection<UserReadModel>("users");
        public IMongoCollection<ChartReadModel> Charts => GetCollection<ChartReadModel>("charts");

        public Task<string> CreateIndexes()
        {
            return Charts.Indexes.CreateOneAsync(Builders<ChartReadModel>.IndexKeys.Geo2DSphere(x => x.Description));
        }
    }
}