
using WeatherForecast.Application.Interfaces.Repositories;
using WeatherForecast.Domain.Entities;

namespace WeatherForecast.Application.Interfaces.Persistence
{
    public interface IWeatherForecastRepository : IAsyncRepository<WeatherForecastEntity>
    {
        Task<IEnumerable<WeatherForecastEntity>> GetWeatherForecastByUserName(string userName);
    }
}
