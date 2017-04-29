using System.Threading.Tasks;
using Geofy.Domain.Events.Chart;
using Geofy.Infrastructure.ServiceBus.Dispatching.Interfaces;
using Geofy.Infrastructure.ServiceBus.Interfaces;
using Geofy.ReadModels;
using Geofy.ReadModels.Services.Databases;
using Geofy.ReadModels.Services.User;
using Geofy.Signals;
using MongoDB.Driver;
using MongoDB.Driver.GeoJsonObjectModel;

namespace Geofy.EventHandlers
{
    public class ChartEventHandler 
        : IMessageHandlerAsync<ChartCreated>
    {
        private readonly IMongoCollection<ChartReadModel> _chartMongoCollection;
        private readonly UserReadModelService _userReadModelService;
        private readonly IMessageBus _messageBus;

        public ChartEventHandler(MongoReadModelsDatabase database, UserReadModelService userReadModelService,
            IMessageBus messageBus)
        {
            _userReadModelService = userReadModelService;
            _messageBus = messageBus;
            _chartMongoCollection = database.Charts;
        }

        public async Task HandleAsync(ChartCreated message)
        {
            var user = await _userReadModelService.GetByIdAsync(message.OwnerId);
            var chart = new ChartReadModel
            {
                Id = message.ChartId,
                Title = message.Title,
                Location = new GeoJsonPoint<GeoJson2DGeographicCoordinates>(
                    new GeoJson2DGeographicCoordinates(message.Longitude, message.Latitude)),
                Radius = message.Radius,
                Description = message.Description,
                OwnerId = user.Id,
                AdminIds = new[] {user.Id},
                Participants = new[] {new Participant {UserId = user.Id, UserName = user.UserName}}
            };
            await _chartMongoCollection.InsertOneAsync(chart);
            await _messageBus.SendRealTimeMessageAsync(new ChartCreatedSignal
            {
                ChartId = chart.Id,
                Title = chart.Title,
                Location =  new Location
                {
                    Latitude = chart.Location.Coordinates.Latitude,
                    Longitude = chart.Location.Coordinates.Longitude
                },
                Radius = chart.Radius,
                Description = chart.Description,
                OwnerId = chart.OwnerId,
                AdminIds = chart.AdminIds,
                Participants = chart.Participants,
                Metadata = new MessageMetadata
                {
                    UserId = message.Metadata.UserId,
                    MessageId = message.Metadata.EventId
                }
            });
        }
    }
}