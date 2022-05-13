using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WeatherForecast.API.Controllers;
using WeatherForecast.Application.Extensions;
using WeatherForecast.Application.Models;
using WeatherForecast.Application.Features.WeatherForecast.Commands.Create;
using WeatherForecast.Application.Features.WeatherForecast.Commands.Delete;
using WeatherForecast.Application.Features.WeatherForecast.Commands.Update;
using WeatherForecast.Application.Features.WeatherForecast.Queries.GetWeatherForecast;
using WeatherForecast.Application.WeatherForecasts.Queries.GetWeatherForecast;
using MassTransit;
using EventBus.Events;
using System.Net;

namespace SimpleCleanArch.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WeatherForecastController : BaseController
    {
        private static readonly string[] Summaries = new[]
        {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;
        readonly IPublishEndpoint _publishEndpoint;
        public WeatherForecastController(ILogger<WeatherForecastController> logger, IPublishEndpoint publishEndpoint)
        {
            _logger = logger;
            _publishEndpoint = publishEndpoint;
        }

        [HttpGet]
        [Route("GetWeatherForecast")]
        //[Authorize(Roles = "Admin")]
        [Authorize(policy: "AccountsAdmin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<PaginatedList<WeatherForecastModel>>> Get([FromQuery] GetWeatherForecastWithPaginationQuery query)
        {
            return Ok(await Mediator.Send(query));
        }

        [HttpGet]
        //[Authorize(policy: "Admin")]
        public async Task<ActionResult<PaginatedList<WeatherForecastModel>>> GetWeatherForecastWithPagination([FromQuery] GetWeatherForecastWithPaginationQuery query)
        {
            return Ok(await Mediator.Send(query));
        }

        [Route("[action]")]
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.Accepted)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        //[Authorize(policy: "SysAdmin")]
        public async Task<IActionResult> Create([FromForm] CreateWeatherForecastCommand command)
        {
            var res = await Mediator.Send(command);

            //Masstransit...
            _publishEndpoint?.Publish(new WeatherForecastCreated(command.TemperatureC, command.Summary, DateTime.UtcNow));

            return Ok(res);
        }

        [HttpPut]
        public async Task<ActionResult> Update([FromForm] UpdateWeatherForecastCommand command)
        {
            await Mediator.Send(command);
            response.Message = "Item updated successfully";
            return Ok(response);
        }

        //[HttpDelete("{id}")]
        [HttpDelete]
        public async Task<IActionResult> Delete([FromQuery] int id)
        {
            await Mediator.Send(new DeleteWeatherForecastCommand { Id = id });
            response.Message = "Item deleted successfully";
            return Ok(response);

        }
    }
}