using Geofy.Infrastructure.ServiceBus.Messages;

namespace Geofy.Domain.Commands.User
{
    public class RegisterUser : Command
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string PasswordHash { get; set; }
        public string PasswordSalt { get; set; }
        public string Email { get; set; }
    }
}