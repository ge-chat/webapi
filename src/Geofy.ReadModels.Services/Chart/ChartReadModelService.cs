using System.Collections.Generic;
using System.Threading.Tasks;
using Geofy.ReadModels.Services.Base;
using Geofy.ReadModels.Services.Databases;
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

        public Task<List<ChartReadModel>> GetInLocationCharts(Location location)
        {
            var builder = Builders<ChartReadModel>.Filter;
            //return _items.Find(builder.Near(x => x.Location,
            //    GeoJson.Point(GeoJson.Geographic(location.Longitude, location.Latitude))))
            //    .ToListAsync();
            //TODO use query by location(not all)
            return _items.Find(FilterDefinition<ChartReadModel>.Empty)
                .ToListAsync();
        }
    }
}