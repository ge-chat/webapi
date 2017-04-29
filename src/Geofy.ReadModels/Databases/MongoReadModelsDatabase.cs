using MongoDB.Driver;

namespace Geofy.ReadModels.Databases
{
    public class MongoReadModelsDatabase
    {
        private readonly MongoInstance _mongo;

        public MongoReadModelsDatabase(string connectionString, bool authenticateToAdmin)
        {
            _mongo = new MongoInstance(connectionString, authenticateToAdmin);
        }

        /// <summary>
        ///     Get database
        /// </summary>
        public IMongoDatabase Database => _mongo.Client.GetDatabase(_mongo.DefaultDatabase);

        protected IMongoCollection<TDocument> GetCollection<TDocument>(string collectionName)
        {
            return Database.GetCollection<TDocument>(collectionName);
        }
    }
}