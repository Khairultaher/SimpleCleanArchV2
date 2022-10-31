using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using WeatherForecast.Application.Common.Mappings;
using WeatherForecast.Application.Extensions;
using WeatherForecast.Application.Interfaces.Persistence;
using WeatherForecast.Application.Models;
using WeatherForecast.Application.WeatherForecasts.Queries.GetWeatherForecast;
using WeatherForecast.Domain.Constants;
using WeatherForecast.Domain.Entities;

namespace WeatherForecast.Application.Features.WeatherForecast.Queries.GetWeatherForecast
{

    public class GetWeatherForecastWithPaginationQuery
        : IRequest<PaginatedList<WeatherForecastModel>>
    {
        //public int ListId { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;

        public string? Location { get; set; }
    }

    public class GetWeatherForecastWithPaginationQueryHandler
        : IRequestHandler<GetWeatherForecastWithPaginationQuery, PaginatedList<WeatherForecastModel>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;
        public GetWeatherForecastWithPaginationQueryHandler(IApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<PaginatedList<WeatherForecastModel>> Handle(GetWeatherForecastWithPaginationQuery request, CancellationToken cancellationToken)
        {
            // view
            var locationTemp = await _context
                .LocationTemperatureSummery
                .Where(x => x.Location == request.Location)
                .ToListAsync();

            var location = new SqlParameter("location", System.Data.SqlDbType.NVarChar);
            location.Value = request.Location;

            // procedure 
            var weatherInfo = await _context
                .GetWeatherInformation
                .FromSqlRaw($"EXEC dbo.{Procedure.GetWeatherInformation} @location", location)
                .ToListAsync();

            // function         
            var temp = (await _context
            .GetTemperatureByLocation
            .FromSqlRaw($"SELECT dbo.{Function.GetTemperatureByLocation}(@location) Temperature", location)
            .FirstOrDefaultAsync())!.Temperature;

            var predicate = PredicateBuilder.True<WeatherForecastEntity>();
            if (!string.IsNullOrEmpty(request.Location))
            {
                predicate = predicate.And(i => i.Location!.ToLower() == request.Location.ToLower());
            }
    
            return await _context.WeatherForecasts
                .Where(predicate)
                .OrderByDescending(x => x.Id)
                .ProjectTo<WeatherForecastModel>(_mapper.ConfigurationProvider)
                .PaginatedListAsync(request.PageNumber, request.PageSize);
        }
    }
}
