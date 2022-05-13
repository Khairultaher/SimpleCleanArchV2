using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models
{
    public record WeatherForecastCreated(int TemperatureC, string? Summary, DateTime CreatedDate);
}
