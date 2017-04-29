using System;
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

        public Task<IEnumerable<ChartReadModel>> GetInLocationCharts()
        {
            throw new NotImplementedException();
        }
    }
}