using Geofy.Domain.Events.Chart;
using Geofy.Infrastructure.Domain;

namespace Geofy.Domain.Chart
{
    public sealed class ChartAggregateState : AggregateState
    {
        public string Id { get; set; }

        public ChartAggregateState()
        {
            On((ChartCreated evt) =>
            {
                Id = evt.ChartId;
            });
        }
    }
}