namespace WeatherForecast.API.ViewModels
{
    public class ResponseModel
    {
        public bool Success { get; set; } = true;
        public string Message { get; set; } = "";
        public Object? Data { get; set; }
        public Object? Error { get; set; }
    }
}
