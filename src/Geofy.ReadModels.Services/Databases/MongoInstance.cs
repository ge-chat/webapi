using System;
using MongoDB.Driver;

namespace Geofy.ReadModels.Services.Databases
{
    public class MongoInstance
    {
        public MongoUrl MongoUrl { get; }

        /// <summary>
        /// Opens connection to MongoDB Client
        /// </summary>
        public MongoInstance(String connectionString, bool authenticateToAdmin)
        {
            var builder = new MongoUrlBuilder(connectionString);
            DefaultDatabase = builder.DatabaseName;
            if (authenticateToAdmin)
                builder.DatabaseName = "admin";
            MongoUrl = builder.ToMongoUrl();
            Client = new MongoClient(MongoUrl);
        }

        public string DefaultDatabase { get; }

        /// <summary>
        /// MongoDB Client
        /// </summary>
        public MongoClient Client { get; }
    }
}
