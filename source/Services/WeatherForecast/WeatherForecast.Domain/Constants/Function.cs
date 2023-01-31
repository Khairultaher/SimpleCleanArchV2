using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeatherForecast.Domain.Constants
{
    public class Function
    {
        /// <summary>
        /// fnGetTemperatureByLocation(@location nvarchar) returns int
        /// </summary>
        public const string GetTemperatureByLocation = "fnGetTemperatureByLocation";
    }
}
