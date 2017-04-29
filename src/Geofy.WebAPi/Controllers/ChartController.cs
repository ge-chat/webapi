using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Geofy.Domain.Commands.Chart;
using Geofy.ReadModels;
using Geofy.ReadModels.Services.Chart;
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
        private readonly ChartReadModelService _chartReadModelService;

        public ChartController(ChartReadModelService chartReadModelService)
        {
            _chartReadModelService = chartReadModelService;
        }

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

        [HttpGet("inlocation")]
        public async Task<List<ChartViewModelShort>> GetCharts(Location location)
        {
            return (await _chartReadModelService.GetInLocationCharts(location))
                .Select(Map)
                .ToList();
        }

        private ChartViewModelShort Map(ChartReadModel model)
        {
            return new ChartViewModelShort
            {
                AdminIds = model.AdminIds,
                Id = model.Id,
                Location = new Location
                {
                    Latitude = model.Location.Coordinates.Latitude,
                    Longitude = model.Location.Coordinates.Longitude
                },
                OwnerId = model.OwnerId,
                Participants = model.Participants,
                Radius = model.Radius,
                Title = model.Title
            };
        }
    }
}