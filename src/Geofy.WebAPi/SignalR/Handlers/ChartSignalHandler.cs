using System.Threading.Tasks;
using Geofy.Infrastructure.ServiceBus.Dispatching.Interfaces;
using Geofy.ReadModels;
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
            _chartHub.Clients.Group(message.Metadata.UserId).chartCreated(new ChartReadModel
            {
                Id = message.ChartId,
                Title = message.Title,
                Location = message.Location,
                Radius = message.Radius,
                Description = message.Description,
                OwnerId = message.OwnerId,
                AdminIds = message.AdminIds,
                Participants = message.Participants
            });
            return Task.CompletedTask;
        }
    }
}