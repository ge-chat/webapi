using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Geofy.Infrastructure.Domain.Transitions;
using Geofy.Infrastructure.Domain.Transitions.Exceptions;
using Geofy.Infrastructure.Domain.Transitions.Interfaces;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Geofy.Infrastructure.Domain.Mongo
{
    public class MongoTransitionRepository : ITransitionRepository
    {
        private const string ConcurrencyException = "E1100";
        private readonly MongoTransitionServer _transitionServer;
        private readonly MongoTransitionSerializer _serializer;

        public MongoTransitionRepository(string connectionString, bool authenticateToAdmin, String transitionsCollectionName = null)
        {
            _serializer = new MongoTransitionSerializer();
            _transitionServer = new MongoTransitionServer(
                InitializeTransitionsDatabase(connectionString, authenticateToAdmin),
                transitionsCollectionName);

            CreateIndexes().Wait();
        }

        private IMongoDatabase InitializeTransitionsDatabase(string connectionString, bool authenticateToAdmin)
        {
            var builder = new MongoUrlBuilder(connectionString);
            if (authenticateToAdmin)
                builder.DatabaseName = "admin";
            var mongoUrl = builder.ToMongoUrl();
            return new MongoClient(mongoUrl).GetDatabase(builder.DatabaseName);
        }

        private Dictionary<BsonDocument, IndexKeysDefinition<BsonDocument>> RequiredIndexes()
        {
            var indexKeys = Builders<BsonDocument>.IndexKeys;
            return new Dictionary<BsonDocument, IndexKeysDefinition<BsonDocument>>
            {
                {new BsonDocument("_id.StreamId", 1), indexKeys.Ascending("_id.StreamId")},
                {new BsonDocument("_id.Version", 1), indexKeys.Ascending("_id.Version")},
                {new BsonDocument("Timestamp", 1), indexKeys.Ascending("Timestamp")},
                {
                    new BsonDocument
                    {
                        new BsonElement("Timestamp", 1),
                        new BsonElement("_id.Version", 1),
                    },
                    indexKeys.Ascending("Timestamp").Ascending("_id.Version")
                }
            };
        }

        public async Task CreateIndexes()
        {
/*            var indexes = _transitionServer.Transitions.GetIndexes().Select(x => x.RawDocument["key"] as BsonDocument).ToList();
            foreach (var index in RequiredIndexes())
            {
                if (!indexes.Contains(index.Key))
                    _transitionServer.Transitions.CreateIndex(index.Value);
            }

            _transitionServer.Snapshots.EnsureIndex(IndexKeys.Ascending("_id.StreamId").Descending("_id.Version"));
            _transitionServer.Snapshots.EnsureIndex(IndexKeys.Ascending("_id.StreamId"));*/
        }

        public async Task AppendTransition(Transition transition)
        {
            // skip saving empty transition
            if (transition.Events.Count < 1)
                return;

            var doc = _serializer.Serialize(transition);

            try
            {
                await _transitionServer.Transitions.InsertOneAsync(doc);
            }
            catch (MongoException e)
            {
                if (!e.Message.Contains(ConcurrencyException))
                    throw;

                throw new DuplicateTransitionException(transition.Id.StreamId, transition.Id.Version, e);
            }
        }

        public async Task AppendTransitions(IEnumerable<Transition> transitions)
        {
            // skip saving empty transition
            var list = transitions.Where(x => x.Events.Any()).ToList();
            if (list.Count < 1)
                return;

            var docs = list.Select(t => _serializer.Serialize(t));

            try
            {
                await _transitionServer.Transitions.InsertManyAsync(docs);
            }
            catch (MongoException e)
            {
                if (!e.Message.Contains(ConcurrencyException))
                    throw;

                throw new DuplicateTransitionException("Several Streams", 0, e);
            }
        }

        public async Task<List<Transition>> GetTransitions(string streamId, int fromVersion, int toVersion)
        {
            var builder = Builders<BsonDocument>.Filter;
            var filter = builder.And(
                builder.Eq("_id.StreamId", streamId),
                builder.Gte("_id.Version", fromVersion),
                builder.Lte("_id.Version", toVersion));
            var sort = Builders<BsonDocument>.Sort.Ascending("_id.Version");

            var docs = await _transitionServer.Transitions.Find(filter).Sort(sort).ToListAsync();

            // Check that such stream exists
            if (docs.Count < 1)
                return new List<Transition>();

            var transitions = docs.Select(_serializer.Deserialize).ToList();

            return transitions;
        }

        public async Task<IEnumerable<Transition>> GetTransitions(int startIndex, int count)
        {
            var sort = Builders<BsonDocument>.Sort
                .Ascending("Timestamp")
                .Ascending("_id.Version");

            var docs = await _transitionServer.Transitions.Find(Builders<BsonDocument>.Filter.Empty)
                .Skip(startIndex)
                .Limit(count)
                .Sort(sort)
                .ToListAsync();

            var transitions = docs.Select(_serializer.Deserialize).ToList();

            return transitions;
        }

        public Task<long> CountTransitions()
        {
            return _transitionServer.Transitions.CountAsync(Builders<BsonDocument>.Filter.Empty);
        }

        /// <summary>
        /// Get all transitions ordered ascendantly by Timestamp of transiton
        /// Should be used only for testing and for very simple event replying 
        /// </summary>
        public async Task<IEnumerable<Transition>> GetTransitions()
        {
            var sort = Builders<BsonDocument>.Sort
                .Ascending("Timestamp")
                .Ascending("_id.Version");

            var docs = await _transitionServer.Transitions.Find(Builders<BsonDocument>.Filter.Empty)
                .Sort(sort).ToListAsync();

            var transitions = docs.Select(_serializer.Deserialize);

            return transitions;
        }

        public Task RemoveTransition(string streamId, int version)
        {
            var id = _serializer.SerializeTransitionId(new TransitionId(streamId, version));
            var query = Builders<BsonDocument>.Filter.Eq("_id", id);

            return _transitionServer.Transitions.DeleteOneAsync(query);
        }

        public Task RemoveStream(String streamId)
        {
            var query = Builders<BsonDocument>.Filter.Eq("_id.StreamId", streamId);

            return _transitionServer.Transitions.DeleteOneAsync(query);
        }
    }
}
