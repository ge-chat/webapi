using System.Security.Claims;
using System.Threading.Tasks;
using AspNet.Security.OpenIdConnect.Extensions;
using Geofy.Infrastructure.ServiceBus.Interfaces;
using Geofy.Shared.Mongo;
using Microsoft.AspNetCore.Mvc;

namespace Geofy.WebAPi.Controllers
{
    public class BaseController : Controller
    {
        protected BaseController(ICommandBus commandBus, IdGenerator idGenerator)
        {
            CommandBus = commandBus;
            IdGenerator = idGenerator;
        }

        protected ICommandBus CommandBus { get; }

        protected IdGenerator IdGenerator { get; }

        protected string UserId => HttpContext.User.GetClaim(ClaimTypes.NameIdentifier);

        protected Task SendAsync(params ICommand[] commands)
        {
            foreach (var command in commands)
            {
                command.Metadata.UserId = UserId;
            }
            return CommandBus.SendAsync(commands);
        }
    }
}