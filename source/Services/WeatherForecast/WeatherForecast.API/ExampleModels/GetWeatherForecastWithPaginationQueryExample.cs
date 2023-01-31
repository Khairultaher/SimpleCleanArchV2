using Swashbuckle.AspNetCore.Filters;
using WeatherForecast.Application.Features.WeatherForecast.Queries.GetWeatherForecast;

namespace WeatherForecast.API.ExampleModels
{
    public class GetWeatherForecastWithPaginationQueryExample: IExamplesProvider<GetWeatherForecastWithPaginationQuery>
    {
        public GetWeatherForecastWithPaginationQuery GetExamples()
        {
            return new GetWeatherForecastWithPaginationQuery
            {
                PageNumber = 1,
                PageSize = 10
            };
        }
    }
}
