using System;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Geofy.Infrastructure.Domain.Mongo
{
    public class MongoTransitionServer
    {
        /// <summary>
        /// Collection for storing commits data
        /// </summary>
        private const string TransitionsCollectionName = "transitions";
        private const string SnapshotsCollectionName = "snapshots";

        public MongoTransitionServer(IMongoDatabase db, String transitionsCollectionName = null)
        {
            transitionsCollectionName = transitionsCollectionName ?? TransitionsCollectionName;

            var options = new MongoCollectionSettings()
            {
                AssignIdOnInsert = false,
                WriteConcern = WriteConcern.Acknowledged
            };

            Transitions = db.GetCollection<BsonDocument>(transitionsCollectionName, options);
            Snapshots = db.GetCollection<BsonDocument>(SnapshotsCollectionName, options);
        }

        /// <summary>
        /// Get commits collection
        /// </summary>
        public IMongoCollection<BsonDocument> Transitions { get; private set; }

        public IMongoCollection<BsonDocument> Snapshots { get; private set; }
    }
}