using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Geofy.ReadModels.Services.Base;
using Geofy.ReadModels.Services.Databases;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Geofy.ReadModels.Services.Chart
{
    public class ChartReadModelService : BaseReadModelServicece<ChartReadModel, ChartFilter>
    {
        public ChartReadModelService(MongoReadModelsDatabase db) :
            base(db.Charts)
        {
        }

        public override IEnumerable<FilterDefinition<ChartReadModel>> BuildFilterQuery(ChartFilter filter)
        {
            yield break;
        }

        //TODO imporove perfomance
        public async Task<List<ChartReadModel>> GetInLocationCharts(Location location)
        {
            var geoNearOptions = new BsonDocument
            {
                {
                    "near", new BsonDocument
                    {
                        {"type", "Point"},
                        {"coordinates", new BsonArray {location.Latitude, location.Longitude}}
                    }
                },
                {"distanceField", "Distanse"},
                {"spherical", true}
            };

            var projectOptions = new BsonDocument
            {
                {"_id", 1},
                {"Radius", 1},
                {"Distanse", 1}
            };

            var pipeline = new List<BsonDocument>
            {
                new BsonDocument {{"$geoNear", geoNearOptions}},
                new BsonDocument {{"$project", projectOptions}}
            };
            var inLocationIds = new List<string>();
            using (var cursor = await _items.AggregateAsync<BsonDocument>(pipeline))
            {
                while (await cursor.MoveNextAsync())
                {
                    inLocationIds.AddRange(
                        from document in cursor.Current
                        where document["Radius"].AsDouble >= document["Distanse"].AsDouble
                        select document["_id"].AsString);
                }
            }

            return await _items.Find(Builders<ChartReadModel>.Filter.In(x => x.Id, inLocationIds))
                .ToListAsync();
        }
    }
}