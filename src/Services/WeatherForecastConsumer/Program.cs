
using Common.Models;
using MassTransit;
using Newtonsoft.Json;


var busControl = Bus.Factory.CreateUsingRabbitMq(cfg =>
{
    cfg.ReceiveEndpoint("WeatherForecast-Created-Event", e =>
    {
        e.Consumer<WeatherForecastCreatedConsumer>();
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
