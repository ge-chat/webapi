using Geofy.Domain.Commands.Chart;
using Geofy.Domain.Events.Chart;
using Geofy.Infrastructure.Domain;

namespace Geofy.Domain.Chart
{
    public class ChartAggregate : Aggregate<ChartAggregateState>
    {
        public void CreateChart(CreateChart cmd)
        {
            Apply(new ChartCreated
            {
                ChartId = cmd.ChartId,
                Latitude = cmd.Latitude,
                Longitude = cmd.Longitude,
                Radius = cmd.Radius,
                Title = cmd.Title,
                OwnerId = cmd.OwnerId,
                Description = cmd.Description
            });
        }

        public void PostMessage(PostMessage cmd)
        {
            Apply(new MessagePosted
            {
                Created = cmd.Created,
                ChartId = cmd.ChartId,
                Message = cmd.Message,
                MessageId = cmd.MessageId,
                UserId = cmd.UserId
            });
        }
    }
}