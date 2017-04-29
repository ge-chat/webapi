using System;
using System.Threading.Tasks;
using Geofy.Domain.Commands.Chart;
using Geofy.Infrastructure.ServiceBus.Interfaces;
using Geofy.Shared.Mongo;
using Geofy.WebAPi.ViewModels.Message;
using Geofy.WebAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Geofy.WebAPi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class MessageController : BaseController
    {
        public MessageController(ICommandBus commandBus, IdGenerator idGenerator) 
            : base(commandBus, idGenerator)
        {
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody]CreateMessagePostModel model)
        {
            if (!ModelState.IsValid) return new GeofyBadRequest(ModelState);
            await SendAsync(new PostMessage
            {
                MessageId = IdGenerator.Generate(),
                ChartId = model.ChartId,
                Created = DateTime.Now,
                Message = model.Message,
                UserId = UserId
            });
            return Ok();
        }
    }
}