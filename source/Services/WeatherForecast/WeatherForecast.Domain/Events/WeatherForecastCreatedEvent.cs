using WeatherForecast.Domain.Common;
using WeatherForecast.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeatherForecast.Domain.Events
{
    public class WeatherForecastCreatedEvent : DomainEvent
    {
        public WeatherForecastCreatedEvent(WeatherForecastEntity item)
        {
            Item = item;
        }

        public WeatherForecastEntity Item { get; }
    }
}
