using Geofy.Infrastructure.ServiceBus.Messages;

namespace Geofy.Domain.Events.User
{
    public class UserCreated : Event
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string PasswordHash { get; set; }
        public string PasswordSalt { get; set; }
        public string Email { get; set; }
    }
}