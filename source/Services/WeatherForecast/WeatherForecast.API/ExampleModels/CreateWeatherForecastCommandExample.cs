using Swashbuckle.AspNetCore.Filters;
using WeatherForecast.Application.Features.WeatherForecast.Commands.Create;
using WeatherForecast.Application.Features.WeatherForecast.Queries.GetWeatherForecast;

namespace WeatherForecast.API.ExampleModels
{
    public class CreateWeatherForecastCommandExample : IExamplesProvider<CreateWeatherForecastCommand>
    {
        public CreateWeatherForecastCommand GetExamples()
        {
            return new CreateWeatherForecastCommand
            {

                       TemperatureC = 33,
                       Location = "Dhaka",
                       Summary = "Hot"
            };
        }
    }
}
