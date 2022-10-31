using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeatherForecast.Domain.Constants
{
    public class Procedure
    {
        /// <summary>
        /// spGetWeatherInformation(@location nvarchar(50))
        /// </summary>
        public const string GetWeatherInformation = "spGetWeatherInformation";
    }
}
