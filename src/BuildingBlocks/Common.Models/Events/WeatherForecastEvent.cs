using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventBus.Events
{
    public record WeatherForecastEvent(int TemperatureC, string? Location, string? Summary, DateTime CreatedDate, string? EventType);
}
