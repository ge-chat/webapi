using Geofy.Domain.Events.User;
using Geofy.Infrastructure.Domain;

namespace Geofy.Domain.User
{
    public sealed class UserAggregateState : AggregateState
    {
        public string Id { get; set; }

        public UserAggregateState()
        {
            On((UserCreated evt) =>
            {
                Id = evt.UserId;
            });
        }
    }
}