using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using WeatherForecast.Application.Common.Mappings;
using WeatherForecast.Application.Interfaces.Persistence;
using WeatherForecast.Application.Models;
using WeatherForecast.Application.WeatherForecasts.Queries.GetWeatherForecast;

namespace WeatherForecast.Application.Features.WeatherForecast.Queries.GetWeatherForecast
{

    public class GetWeatherForecastWithPaginationQuery
        : IRequest<PaginatedList<WeatherForecastModel>>
    {
        //public int ListId { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
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
            return await _context.WeatherForecasts
                    .OrderByDescending(x => x.Id)
                    .ProjectTo<WeatherForecastModel>(_mapper.ConfigurationProvider)
                    .PaginatedListAsync(request.PageNumber, request.PageSize);


        }
    }
}
