using System.Linq;
using System.Threading.Tasks;
using Geofy.Infrastructure.ServiceBus.Dispatching.Interfaces;
using Geofy.Signals;
using Geofy.WebAPi.SignalR.Hubs;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Infrastructure;

namespace Geofy.WebAPi.SignalR.Handlers
{
    public class ChartSignalHandler
        :IMessageHandlerAsync<ChartCreatedSignal>
    {
        private readonly IHubContext _chartHub;

        public ChartSignalHandler(IConnectionManager manager)
        {
            _chartHub = manager.GetHubContext<ChartHub>();
        }

        public Task HandleAsync(ChartCreatedSignal message)
        {
            _chartHub.Clients.Group(message.Metadata.UserId).chartCreated(Map(message));
            return Task.CompletedTask;
        }

        //Convert to camelcase due to SignalR configuration mismatch
        private object Map(ChartCreatedSignal message)
        {
            return new
            {
                id = message.ChartId,
                title = message.Title,
                location = new
                {
                    longitude = message.Location.Longitude,
                    latitude = message.Location.Latitude
                },
                radius = message.Radius,
                description = message.Description,
                ownerId = message.OwnerId,
                adminIds = message.AdminIds,
                participants = message.Participants.Select(x => new {userId = x.UserId, userName = x.UserName})
            };
        }
    }
}