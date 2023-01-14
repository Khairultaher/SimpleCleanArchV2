namespace WeatherForecast.Application.WeatherForecasts.Queries.GetWeatherForecast
{
    public class WeatherForecastModel //: IMapFrom<WeatherForecastEntity>
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }

        public int TemperatureC { get; set; }

        public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);

        public string? Summary { get; set; }
        public string? Location { get; set; }
        //public void Mapping(Profile profile)
        //{
        //    profile.CreateMap<WeatherForecastEntity, WeatherForecastModel>();
        //}
    }
}
