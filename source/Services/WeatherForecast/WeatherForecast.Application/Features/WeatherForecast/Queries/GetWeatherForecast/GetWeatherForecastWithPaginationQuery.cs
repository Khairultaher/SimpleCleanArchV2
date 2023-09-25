using AutoMapper;
using Janus.Application.Extensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using WeatherForecast.Application.Extensions;
using WeatherForecast.Application.Interfaces.Persistence;
using WeatherForecast.Application.WeatherForecasts.Queries.GetWeatherForecast;
using WeatherForecast.Domain.Entities;

namespace WeatherForecast.Application.Features.WeatherForecast.Queries.GetWeatherForecast
{

    public class GetWeatherForecastWithPaginationQuery
        : IRequest<PagedList<WeatherForecastModel>>
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string OrderBy { get; set; } = "Location asc";
        public string? Location { get; set; } = null;
    }

    public class GetWeatherForecastWithPaginationQueryHandler
        : IRequestHandler<GetWeatherForecastWithPaginationQuery, PagedList<WeatherForecastModel>>
    {
        private readonly IApplicationReadDbContext _context;
        public GetWeatherForecastWithPaginationQueryHandler(IApplicationReadDbContext context, IMapper mapper)
        {
            _context = context;
        }
        public async Task<PagedList<WeatherForecastModel>> Handle(GetWeatherForecastWithPaginationQuery request, CancellationToken cancellationToken)
        {
            // view
            //var locationTemp = await _context
            //    .LocationTemperatureSummery
            //    .Where(x => x.Location == request.Location)
            //    .ToListAsync();

            //var location = new SqlParameter("location", System.Data.SqlDbType.NVarChar);
            //location.Value = request.Location;

            // procedure 
            //var weatherInfo = await _context
            //    .GetWeatherInformation
            //    .FromSqlRaw($"EXEC dbo.{Procedure.GetWeatherInformation} @location", location)
            //    .ToListAsync();

            // function         
            //var temp = (await _context
            //.GetTemperatureByLocation
            //.FromSqlRaw($"SELECT dbo.{Function.GetTemperatureByLocation}(@location) Temperature", location)
            //.FirstOrDefaultAsync())!.Temperature;

            var predicate = PredicateBuilder.True<WeatherForecastEntity>();
            if (!string.IsNullOrEmpty(request.Location))
            {
                predicate = predicate.And(i => i.Location!.ToLower() == request.Location.ToLower());
            }



            return await _context.WeatherForecasts
                .Where(predicate)
                .OrderByDescending(x => x.Id)
                //.ProjectTo<WeatherForecastModel>(_mapper.ConfigurationProvider)
                .Select(x => new WeatherForecastModel
                {
                    Id = x.Id,
                    Location = x.Location,
                    Summary = x.Summary,
                    Date = x.Date,
                    TemperatureC = x.TemperatureC
                })
                .ToPagedAsync(request.PageNumber, request.PageSize, request.OrderBy);
        }
    }
}
