using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WeatherForecast.API.Controllers;
using WeatherForecast.Application.Common.Extensions;
using WeatherForecast.Application.Common.Models;
using WeatherForecast.Application.WeatherForecasts.Commands;
using WeatherForecast.Application.WeatherForecasts.Queries;

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

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        [Route("GetWeatherForecast")]
        //[Authorize(Roles = "Admin")]
        [Authorize(policy: "AccountsAdmin")]
        public async Task<ActionResult<PaginatedList<WeatherForecastModel>>> Get([FromQuery] GetWeatherForecastWithPaginationQuery query)
        {

            return await Mediator.Send(query);

            //try
            //{

            //    await Task.Delay(100);
            //    var data = await Task.FromResult(Enumerable.Range(1, 20).Select(index => new WeatherForecast
            //    {
            //        Date = DateTime.Now.AddDays(index),
            //        TemperatureC = Random.Shared.Next(-20, 55),
            //        Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            //    })
            //    .ToList());

            //    var res = new { data = data.Skip(skip - 1).Take(take).ToList(), count = data.Count() };

            //    return Ok(res);
            //}
            //catch (Exception ex)
            //{
            //    var res = new { message = ex.Message };
            //    return BadRequest(res);
            //}
        }

        [HttpGet]
        [Authorize(policy: "SysAdmin")]
        public async Task<ActionResult<PaginatedList<WeatherForecastModel>>> GetWeatherForecastWithPagination([FromQuery] GetWeatherForecastWithPaginationQuery query)
        {
            return await Mediator.Send(query);
        }

        [HttpPost]
        [Authorize(policy: "SysAdmin")]
        public async Task<IActionResult> Create([FromForm] CreateWeatherForecastCommand command)
        {
            try
            {
                return Ok(await Mediator.Send(command));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.GetExceptions());
            }
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromForm] UpdateWeatherForecastCommand command)
        {
            try
            {
                await Mediator.Send(command);
                response.Message = "Item updated successfully";
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.GetExceptions());
            }
        }

        //[HttpDelete("{id}")]
        [HttpDelete]
        public async Task<IActionResult> Delete([FromQuery] int id)
        {
            try
            {
                await Mediator.Send(new DeleteWeatherForecastCommand { Id = id });
                response.Message = "Item deleted successfully";
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.GetExceptions());
            }
        }
    }
}