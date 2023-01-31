using WeatherForecast.Domain.Common;
using WeatherForecast.Domain.Entities;

namespace WeatherForecast.Domain.Events
{
    public class WeatherForecastDeletedEvent : DomainEvent
    {
        public WeatherForecastDeletedEvent(WeatherForecastEntity item)
        {
            Item = item;
        }

        public WeatherForecastEntity Item { get; }
    }
}
