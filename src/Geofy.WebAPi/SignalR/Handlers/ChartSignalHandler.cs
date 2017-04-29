using System.Linq;
using System.Threading.Tasks;
using Geofy.Infrastructure.ServiceBus.Dispatching.Interfaces;
using Geofy.Signals;
using Geofy.WebAPi.SignalR.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Infrastructure;

namespace Geofy.WebAPi.SignalR.Handlers
{
    public class ChartSignalHandler :
        IMessageHandlerAsync<ChartCreatedSignal>,
        IMessageHandlerAsync<MessagePostedSignal>,
        IMessageHandlerAsync<ParticipantAddedSignal>
    {
        private readonly IHubContext _chartHub;
        private readonly IHubContext _userHub;

        public ChartSignalHandler(IConnectionManager manager)
        {
            _userHub = manager.GetHubContext<UserHub>();
            _chartHub = manager.GetHubContext<ChartHub>();
        }

        public Task HandleAsync(ChartCreatedSignal message)
        {
            _userHub.Clients.Group(message.Metadata.UserId).chartCreated(Map(message));
            return Task.CompletedTask;
        }

        public Task HandleAsync(MessagePostedSignal message)
        {
            _chartHub.Clients.Group(message.ChartId).messagePosted(Map(message));
            return Task.CompletedTask;
        }

        public Task HandleAsync(ParticipantAddedSignal message)
        {
            _chartHub.Clients.Group(message.ChartId).participantAdded(Map(message));
            return Task.CompletedTask;
        }

        #region Convert to camelcase due to SignalR configuration mismatch
        private object Map(ParticipantAddedSignal message)
        {
            return new
            {
                chartId = message.ChartId,
                participant = new
                {
                    userId = message.Participant.UserId,
                    userName = message.Participant.UserName
                }
            };
        }

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
                participants = message.Participants.Select(x => new {userId = x.UserId, userName = x.UserName}),
                messages = new object[0]
            };
        }

        private object Map(MessagePostedSignal message)
        {
            return new
            {
                id = message.MessageId,
                chartId = message.ChartId,
                created = message.Created,
                message = message.Message,
                userId = message.UserId
            };
        }
        #endregion
    }
}