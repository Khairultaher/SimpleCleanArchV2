using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#nullable disable

namespace WeatherForecast.Domain.Dtos
{
    public class LocationTemperatureSummeryDto
    {
        public DateTime Date { get; set; }
        public string Location { get; set; }
        public int Temperature { get; set; }
        public string Summary { get; set; }
    }
}
