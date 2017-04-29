using System.Threading.Tasks;
using Geofy.Infrastructure.ServiceBus.Interfaces;
using Geofy.Shared.Mongo;
using Microsoft.AspNet.Mvc;

namespace Geofy.WebAPi.Controllers
{
    public class BaseController : Controller
    {
        [FromServices]
        public ICommandBus CommandBus { get; set; }

        [FromServices]
        public IdGenerator IdGenerator { get; set; }

        public string UserId => null;

        public Task SendAsync(params ICommand[] commands)
        {
            foreach (var command in commands)
            {
                command.Metadata.UserId = UserId;
            }
            return CommandBus.SendAsync(commands);
        }
    }
}