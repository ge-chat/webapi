using System.Threading.Tasks;
using Geofy.Domain.Commands.User;
using Geofy.Infrastructure.Domain.Interfaces;
using Geofy.Infrastructure.ServiceBus.Dispatching.Interfaces;

namespace Geofy.Domain.User
{
    public class UserCommandService
        : IMessageHandlerAsync<RegisterUser>
    {
        private readonly IRepository<UserAggregate> _repository;

        public UserCommandService(IRepository<UserAggregate> repository)
        {
            _repository = repository;
        } 

        public async Task HandleAsync(RegisterUser message)
        {
            await _repository.Perform(message.UserId, agr => agr.RegisterUser(message));
        }
    }
}