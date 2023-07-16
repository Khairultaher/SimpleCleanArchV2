using MediatR;
using WeatherForecast.Application.Exceptions;
using WeatherForecast.Application.Interfaces.Persistence;
using WeatherForecast.Domain.Entities;

namespace WeatherForecast.Application.Features.WeatherForecast.Commands.Update
{
    public class UpdateWeatherForecastCommand : IRequest
    {
        public int Id { get; set; }
        public int TemperatureC { get; set; }
        public string? Location { get; set; }
        public string? Summary { get; set; }
    }

    public class UpdateWeatherForecastCommandHander : IRequestHandler<UpdateWeatherForecastCommand>
    {
        private readonly IApplicationWriteDbContext _context;
        public UpdateWeatherForecastCommandHander(IApplicationWriteDbContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(UpdateWeatherForecastCommand request, CancellationToken cancellationToken)
        {
            WeatherForecastEntity? entity = await _context.WeatherForecasts.FindAsync(new object[] { request.Id }, cancellationToken);

            if (entity == null)
            {
                throw new NotFoundException(nameof(WeatherForecast), request.Id);
            }

            entity.TemperatureC = request.TemperatureC;
            entity.Location = request.Location;
            entity.Summary = request.Summary;


            await _context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
