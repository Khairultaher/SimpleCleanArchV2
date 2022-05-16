namespace EventBus.Common
{
    public static class EventBusConstants
    {

        public static class Exchages
        {
            public static string WeatherForecastExchange = "WeatherForecast-Exchange";
        }

        public static class Queues
        {
            public static string WeatherForecastCreatedQueue = "WeatherForecastCreated-Queue";
            public static string WeatherForecastUpdatedQueue = "WeatherForecastUpdated-Queue";
        }


        public static class RabbitMQSettings
        {
            public static string Host = "localhost";
            public static string HostAddress = "amqp://guest:guest@localhost:15672";
        }
    }
}
