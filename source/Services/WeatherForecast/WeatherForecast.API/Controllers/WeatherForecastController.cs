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
using Swashbuckle.AspNetCore.Filters;
using WeatherForecast.API.ExampleModels;
using Janus.Application.Extensions;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System.Text;

namespace SimpleCleanArch.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "WF", IgnoreApi = false)]
    public class WeatherForecastController : BaseController
    {
        private static readonly string[] Summaries = new[]
        {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };
        private readonly IDistributedCache _distributedCache;
        private readonly ILogger<WeatherForecastController> _logger;
        readonly IPublishEndpoint _publishEndpoint;
        private const string cacheKey = "weatherforecast-get-1";
        public WeatherForecastController(ILogger<WeatherForecastController> logger
            , IPublishEndpoint publishEndpoint, IDistributedCache distributedCache
            )
        {
            _logger = logger;
            _publishEndpoint = publishEndpoint;
            _distributedCache = distributedCache;
        }

        /// <summary>
        /// Get Weather Forecast 
        /// </summary>
        /// <param name="query"></param>
        /// <returns>Return success/fail status</returns>
        /// <remarks>
        /// **Sample request body:**
        ///
        ///     {
        ///        "PageNumber": 1,
        ///        "PageSize": 10,
        ///     }
        ///
        /// </remarks>
        /// <response code="200">Success</response>
        /// <response code="401">Failed/Unauthorized</response>
        [HttpGet]
        //[Authorize(Roles = "Admin")]
        //[Authorize(policy: "AccountsAdmin")]
        //[ProducesResponseType(StatusCodes.Status200OK)]
        //[ProducesDefaultResponseType]
        public async Task<ActionResult<PagedList<WeatherForecastModel>>> Get([FromQuery] GetWeatherForecastWithPaginationQuery query)
        {          
            var encodedData = await _distributedCache.GetAsync(cacheKey);
            if (encodedData != null)
            {
                var serializedBooks = Encoding.UTF8.GetString(encodedData);
                var data = JsonConvert.DeserializeObject<PagedList<WeatherForecastModel>>(serializedBooks);

                return Ok(data);
            }
            else
            {
                var data = await Mediator.Send(query);
                var serializedData = JsonConvert.SerializeObject(data);
                encodedData = Encoding.UTF8.GetBytes(serializedData);

                //var options = new DistributedCacheEntryOptions()
                //                .SetSlidingExpiration(TimeSpan.FromMinutes(2))
                //                .SetAbsoluteExpiration(TimeSpan.FromMinutes(2));
                //await _distributedCache.SetAsync(cacheKey, encodedData, options);
                await _distributedCache.SetAsync(cacheKey, encodedData);

                return Ok(data);
            }
        }

        /// <summary>
        /// Get Weather Forecast 
        /// </summary>
        /// <param name="pageNumber" example="1" ></param>
        /// <param name="pageSize" example="10" ></param>
        ///  <param name="orderBy" example="Date asc" ></param>
        /// <returns>Return success/fail status</returns>
        /// <remarks>
        /// **Sample request body:**
        ///
        ///     {
        ///        "PageNumber": 1,
        ///        "PageSize": 10
        ///        "OrderBy": "Date asc"
        ///     }
        ///
        /// </remarks>
        /// <response code="200">Success</response>
        /// <response code="401">Failed/Unauthorized</response>
        [HttpGet]
        [Route("GetWeatherForecast")]
        [Authorize(policy: "Admin")]
        //[ProducesResponseType(StatusCodes.Status200OK)]
        //[ProducesDefaultResponseType]
        public async Task<ActionResult<PagedList<WeatherForecastModel>>> GetWeatherForecastWithPagination([FromQuery] int pageNumber, int pageSize, string orderBy)
        {
            return Ok(await Mediator.Send(new GetWeatherForecastWithPaginationQuery { PageNumber = pageNumber, PageSize = pageSize, OrderBy = orderBy }));
        }

        /// <summary>
        /// Add Weather Forecast
        /// </summary>
        /// <param name="command" example=""></param>
        /// <returns>Return success/fail status</returns>
        /// <remarks>
        /// **Sample request body:**
        ///
        ///     {
        ///        "TemperatureC": 30,
        ///        "Location": "Dhaka",
        ///        "Summary": "Hot"
        ///     }
        ///
        /// </remarks>
        /// <response code="200">Success</response>
        /// <response code="401">Failed/Unauthorized</response>
        [HttpPost]
        //[Authorize(policy: "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesDefaultResponseType]
        //[SwaggerRequestExample(typeof(CreateWeatherForecastCommand), typeof(CreateWeatherForecastCommandExample))]
        public async Task<IActionResult> Create([FromBody] CreateWeatherForecastCommand command)
        {
            response.Data = await Mediator.Send(command);
            response.Message = "Item Added successfully";
            await _distributedCache.RemoveAsync(cacheKey);

            //Masstransit...
            _publishEndpoint?.Publish(new WeatherForecastEvent(command.TemperatureC, command.Location, command.Summary, DateTime.UtcNow, EventBusEnums.CREATED.ToString()));

            return Ok(response);
        }


        /// <summary>
        /// Update Weather Forecast
        /// </summary>
        /// <param name="command"></param>
        /// <returns>Return success/fail status</returns>
        /// <remarks>
        /// **Sample request body:**
        ///
        ///     {
        ///        "Id": 12,
        ///        "TemperatureC": 30,
        ///        "Location": "Dhaka",
        ///        "Summary": "Hot"
        ///     }
        ///
        /// </remarks>
        /// <response code="200">Success</response>
        /// <response code="401">Failed/Unauthorized</response>
        [HttpPut]
        //[Authorize(policy: "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesDefaultResponseType]
        //[SwaggerRequestExample(typeof(UpdateWeatherForecastCommand), typeof(UpdateWeatherForecastCommandExample))]
        public async Task<ActionResult> Update([FromBody] UpdateWeatherForecastCommand command)
        {
            response.Data = await Mediator.Send(command);
            response.Message = "Item updated successfully";

            //Masstransit...
            _publishEndpoint?.Publish(new WeatherForecastEvent(command.TemperatureC, command.Location, command.Summary, DateTime.UtcNow, EventBusEnums.UPDATED.ToString()));

            return Ok(response);
        }


        /// <param name="id" example="123">The product id</param>
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