namespace EventBus.Common
{
    public static class EventBusConstants
    {
        public static string WeatherForecastCreatedQueue = "WeatherForecastCreated-Queue";
        public static class RabbitMQSettings
        {
            public static string Host = "localhost";
            public static string HostAddress = "amqp://guest:guest@localhost:15672";
        }
    }
}
