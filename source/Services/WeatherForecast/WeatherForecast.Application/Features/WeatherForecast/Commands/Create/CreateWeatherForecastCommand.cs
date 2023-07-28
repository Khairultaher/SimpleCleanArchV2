using MediatR;
using WeatherForecast.Application.Interfaces.Persistence;
using WeatherForecast.Domain.Entities;
using WeatherForecast.Domain.Events;

namespace WeatherForecast.Application.Features.WeatherForecast.Commands.Create
{
    public class CreateWeatherForecastCommand : IRequest<int>
    {
        public int TemperatureC { get; set; }
        public string? Location { get; set; }
        public string? Summary { get; set; }
    }

    public class CreateWeatherForecastCommandHandler : IRequestHandler<CreateWeatherForecastCommand, int>
    {
        private readonly IApplicationWriteDbContext _context;
        public CreateWeatherForecastCommandHandler(IApplicationWriteDbContext context)
        {
            _context = context;
        }
        public async Task<int> Handle(CreateWeatherForecastCommand request, CancellationToken cancellationToken)
        {
            var entity = new WeatherForecastEntity
            {
                TemperatureC = request.TemperatureC,
                Location = request.Location,
                CreatedAt = DateTime.UtcNow,
                Date = DateTime.UtcNow,
                Summary = request.Summary,
            };

            entity.DomainEvents.Add(new WeatherForecastCreatedEvent(entity));

            _context.WeatherForecasts.Add(entity);

            await _context.SaveChangesAsync(cancellationToken);

            return entity.Id;
        }
    }
}
