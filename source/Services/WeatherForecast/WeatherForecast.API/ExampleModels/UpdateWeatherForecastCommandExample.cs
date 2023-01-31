using Swashbuckle.AspNetCore.Filters;
using WeatherForecast.Application.Features.WeatherForecast.Commands.Create;
using WeatherForecast.Application.Features.WeatherForecast.Commands.Update;
using WeatherForecast.Application.Features.WeatherForecast.Queries.GetWeatherForecast;

namespace WeatherForecast.API.ExampleModels
{
    public class UpdateWeatherForecastCommandExample : IExamplesProvider<UpdateWeatherForecastCommand>
    {
        public UpdateWeatherForecastCommand GetExamples()
        {
            return new UpdateWeatherForecastCommand
            {
                Id = 1,
                TemperatureC = 33,
                Location = "Dhaka",
                Summary = "Hot"
            };
        }
    }
}
