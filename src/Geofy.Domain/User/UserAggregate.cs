using Geofy.Domain.Commands.User;
using Geofy.Domain.Events.User;
using Geofy.Infrastructure.Domain;

namespace Geofy.Domain.User
{
    public class UserAggregate : Aggregate<UserAggregateState>
    {
        public void RegisterUser(RegisterUser cmd)
        {
            Apply(new UserCreated
            {
                UserId = State.Id,
                Email = cmd.Email,
                PasswordHash = cmd.PasswordHash,
                PasswordSalt = cmd.PasswordSalt,
                UserName = cmd.UserName
            });
        }
    }
}