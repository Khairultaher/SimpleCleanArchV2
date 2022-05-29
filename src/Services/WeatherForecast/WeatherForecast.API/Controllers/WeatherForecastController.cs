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
using EventBus.Common;

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
        public WeatherForecastController(ILogger<WeatherForecastController> logger
            , IPublishEndpoint publishEndpoint
            )
        {
            _logger = logger;
            _publishEndpoint = publishEndpoint;
        }

        [HttpGet]
        [Route("GetWeatherForecast")]
        //[Authorize(Roles = "Admin")]
        //[Authorize(policy: "AccountsAdmin")]
        //[ProducesResponseType(StatusCodes.Status200OK)]
        //[ProducesDefaultResponseType]
        public async Task<ActionResult<PaginatedList<WeatherForecastModel>>> Get([FromQuery] GetWeatherForecastWithPaginationQuery query)
        {
            return Ok(await Mediator.Send(query));
        }

        [HttpGet]
        //[Authorize(policy: "Admin")]
        //[ProducesResponseType(StatusCodes.Status200OK)]
        //[ProducesDefaultResponseType]
        public async Task<ActionResult<PaginatedList<WeatherForecastModel>>> GetWeatherForecastWithPagination([FromQuery] GetWeatherForecastWithPaginationQuery query)
        {
            return Ok(await Mediator.Send(query));
        }

        [HttpPost]
        //[Authorize(policy: "Admin")]
        //[ProducesResponseType(StatusCodes.Status200OK)]
        //[ProducesDefaultResponseType]
        public async Task<IActionResult> Create([FromForm] CreateWeatherForecastCommand command)
        {
            response.Data = await Mediator.Send(command);
            response.Message = "Item Added successfully";

            //Masstransit...
            _publishEndpoint?.Publish(new WeatherForecastEvent(command.TemperatureC, command.Location, command.Summary, DateTime.UtcNow, EventBusEnums.CREATED.ToString()));

            return Ok(response);
        }


        [HttpPut]
        //[Authorize(policy: "Admin")]
        //[ProducesResponseType(StatusCodes.Status200OK)]
        //[ProducesDefaultResponseType]
        public async Task<ActionResult> Update([FromForm] UpdateWeatherForecastCommand command)
        {
            response.Data = await Mediator.Send(command);
            response.Message = "Item updated successfully";

            //Masstransit...
            _publishEndpoint?.Publish(new WeatherForecastEvent(command.TemperatureC, command.Location, command.Summary, DateTime.UtcNow, EventBusEnums.UPDATED.ToString()));

            return Ok(response);
        }


        [HttpDelete]
        //[Authorize(policy: "Admin")]
        //[ProducesResponseType(StatusCodes.Status200OK)]
        //[ProducesDefaultResponseType]
        public async Task<IActionResult> Delete([FromQuery] int id)
        {

            return Ok(await Mediator.Send(new DeleteWeatherForecastCommand { Id = id }));

        }
    }
}