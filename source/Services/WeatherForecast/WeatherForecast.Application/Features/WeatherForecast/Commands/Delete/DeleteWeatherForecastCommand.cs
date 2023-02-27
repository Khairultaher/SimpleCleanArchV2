using MediatR;
using WeatherForecast.Application.Exceptions;
using WeatherForecast.Application.Interfaces.Persistence;
using WeatherForecast.Domain.Events;

namespace WeatherForecast.Application.Features.WeatherForecast.Commands.Delete
{
    public class DeleteWeatherForecastCommand : IRequest
    {
        public int Id { get; set; }
    }
    public class DeleteWeatherForecastCommandHandler : IRequestHandler<DeleteWeatherForecastCommand>
    {
        private readonly IApplicationDbContext _context;
        public DeleteWeatherForecastCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(DeleteWeatherForecastCommand request, CancellationToken cancellationToken)
        {
            var entity = await _context.WeatherForecasts.FindAsync(new object[] { request.Id }, cancellationToken);

            if (entity == null)
            {
                throw new NotFoundException(nameof(WeatherForecast), request.Id);
            }

            _context.WeatherForecasts.Remove(entity);

            entity.DomainEvents.Add(new WeatherForecastDeletedEvent(entity));

            await _context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
