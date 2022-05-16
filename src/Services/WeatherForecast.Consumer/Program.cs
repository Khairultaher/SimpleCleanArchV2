
using EventBus.Common;
using EventBus.Events;
using MassTransit;
using Newtonsoft.Json;
using RabbitMQ.Client;


var busControl = Bus.Factory.CreateUsingRabbitMq(cfg =>
{
    cfg.ReceiveEndpoint(EventBusConstants.Queues.WeatherForecastCreatedQueue, e =>
    {
        // turns off default fanout settings
        e.ConfigureConsumeTopology = false;
        // a replicated queue to provide high availability and data safety. available in RMQ 3.8+
        //e.SetQuorumQueue();
        //e.SetQueueArgument("declare", "lazy");


        e.Consumer<WeatherForecastCreatedConsumer>();
        e.Bind(EventBusConstants.Exchages.WeatherForecastExchange, s =>
        {
            s.RoutingKey = EventBusEnums.CREATED.ToString();
            s.ExchangeType = ExchangeType.Direct;
        });

        e.PrefetchCount = 20;
        e.UseMessageRetry(r => r.Interval(2, 100));

    });

    cfg.ReceiveEndpoint(EventBusConstants.Queues.WeatherForecastUpdatedQueue, e =>
    {
        // turns off default fanout settings
        e.ConfigureConsumeTopology = false;
        // a replicated queue to provide high availability and data safety. available in RMQ 3.8+
        //e.SetQuorumQueue();
        //e.SetQueueArgument("declare", "lazy");

        e.Consumer<WeatherForecastUpdatedConsumer>();
        e.Bind("WeatherForecast-Exchange", s =>
        {
            s.RoutingKey = EventBusEnums.UPDATED.ToString();
            s.ExchangeType = ExchangeType.Direct;
        });

        e.PrefetchCount = 20;
        e.UseMessageRetry(r => r.Interval(2, 100));

    });
});

await busControl.StartAsync(new CancellationToken());

try
{
    Console.WriteLine("Press enter to exit");

    await Task.Run(() => Console.ReadLine());
}
finally
{
    await busControl.StopAsync();
}

class WeatherForecastCreatedConsumer : IConsumer<WeatherForecastEvent>
{
    public async Task Consume(ConsumeContext<WeatherForecastEvent> context)
    {
        await Task.Run(() =>
        {
            var jsonMessage = JsonConvert.SerializeObject(context.Message);
            Console.WriteLine($"New weather forecast for {context.Message.Location} is added: {jsonMessage}");
        });
    }
}

class WeatherForecastUpdatedConsumer : IConsumer<WeatherForecastEvent>
{
    public async Task Consume(ConsumeContext<WeatherForecastEvent> context)
    {
        await Task.Run(() =>
        {
            var jsonMessage = JsonConvert.SerializeObject(context.Message);
            Console.WriteLine($"Weather forecast for {context.Message.Location} is updated : {jsonMessage}");
        });
    }
}
