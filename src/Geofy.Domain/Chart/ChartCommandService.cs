using System.Threading.Tasks;
using Geofy.Domain.Commands.Chart;
using Geofy.Domain.User;
using Geofy.Infrastructure.Domain.Interfaces;
using Geofy.Infrastructure.ServiceBus.Dispatching.Interfaces;

namespace Geofy.Domain.Chart
{
    public class ChartCommandService :
        IMessageHandlerAsync<CreateChart>
    {
        private readonly IRepository<ChartAggregate> _repository;

        public ChartCommandService(IRepository<ChartAggregate> repository)
        {
            _repository = repository;
        }

        public Task HandleAsync(CreateChart message)
        {
            return _repository.Perform(message.ChartId, x => x.CreateChart(message), message.Metadata);
        }
    }
}