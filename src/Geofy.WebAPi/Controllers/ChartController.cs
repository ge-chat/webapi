using System.Threading.Tasks;
using Geofy.Domain.Commands.Chart;
using Geofy.WebAPi.ViewModels.Chart;
using Geofy.WebAPI.Services;
using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Mvc;

namespace Geofy.WebAPi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class ChartController : BaseController
    {
        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody]CreateChartViewModel model)
        {
            if(!ModelState.IsValid) return new GeofyBadRequest(ModelState);
            await SendAsync(new CreateChart
            {
                ChartId = IdGenerator.Generate(),
                Title = model.Title,
                Latitude = model.Latitude,
                Longitude = model.Longitude,
                Radius = model.Radius,
                OwnerId = UserId,
                Description = model.Description
            });
            return Ok();
        }
    }
}