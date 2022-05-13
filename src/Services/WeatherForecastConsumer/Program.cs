
using EventBus.Common;
using EventBus.Events;
using MassTransit;
using Newtonsoft.Json;


//var builder = WebApplication.CreateBuilder(args);

var busControl = Bus.Factory.CreateUsingRabbitMq(cfg =>
{
    //cfg.Host("localhost", "/", h =>
    //{
    //    h.Username("guest");
    //    h.Password("guest");
    //});
    cfg.ReceiveEndpoint(EventBusConstants.WeatherForecastCreatedQueue, e =>
    {
        e.Consumer<WeatherForecastCreatedConsumer>();
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

class WeatherForecastCreatedConsumer : IConsumer<WeatherForecastCreated>
{
    public async Task Consume(ConsumeContext<WeatherForecastCreated> context)
    {
        await Task.Run(() =>
        {
            var jsonMessage = JsonConvert.SerializeObject(context.Message);
            Console.WriteLine($"WeatherForecastCreated message: {jsonMessage}");
        });
    }
}
