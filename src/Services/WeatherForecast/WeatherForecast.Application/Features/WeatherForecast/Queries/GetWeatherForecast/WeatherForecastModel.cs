using AutoMapper;
using WeatherForecast.Application.Common.Mappings;
using WeatherForecast.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeatherForecast.Application.Mappings;

namespace WeatherForecast.Application.WeatherForecasts.Queries.GetWeatherForecast
{
    public class WeatherForecastModel : IMapFrom<WeatherForecastEntity>
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }

        public int TemperatureC { get; set; }

        public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);

        public string? Summary { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<WeatherForecastEntity, WeatherForecastModel>();
        }
    }
}
