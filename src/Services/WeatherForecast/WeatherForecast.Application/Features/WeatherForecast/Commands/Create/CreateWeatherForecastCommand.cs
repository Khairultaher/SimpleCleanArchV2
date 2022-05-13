using MediatR;
using WeatherForecast.Application.Interfaces;
using WeatherForecast.Domain.Entities;
using WeatherForecast.Domain.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using WeatherForecast.Application.Interfaces.Persistence;

namespace WeatherForecast.Application.Features.WeatherForecast.Commands.Create
{
    public class CreateWeatherForecastCommand : IRequest<int>
    {
        public int TemperatureC { get; set; }

        public string? Summary { get; set; }
    }

    public class CreateWeatherForecastCommandHandler : IRequestHandler<CreateWeatherForecastCommand, int>
    {
        private readonly IApplicationDbContext _context;
        public CreateWeatherForecastCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<int> Handle(CreateWeatherForecastCommand request, CancellationToken cancellationToken)
        {
            var entity = new WeatherForecastEntity
            {
                TemperatureC = request.TemperatureC,
                CreatedAt = DateTime.UtcNow,
                Summary = request.Summary,
            };

            entity.DomainEvents.Add(new WeatherForecastCreatedEvent(entity));

            _context.WeatherForecasts.Add(entity);

            await _context.SaveChangesAsync(cancellationToken);

            return entity.Id;
        }
    }
}
