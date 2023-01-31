using AutoMapper;
using Microsoft.EntityFrameworkCore;
using WeatherForecast.Application.Common.Mappings;
using WeatherForecast.Application.Features.WeatherForecast.Queries.GetWeatherForecast;
using WeatherForecast.Application.Interfaces.Persistence;
using WeatherForecast.Application.Mappings;
using WeatherForecast.Application.Models;
using WeatherForecast.Application.WeatherForecasts.Queries.GetWeatherForecast;
using WeatherForecast.Domain.Entities;

namespace WeatherForecast.Application.Tests
{
    public class GetWeatherForecastWithPaginationQueryHandlerTest
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;
        public GetWeatherForecastWithPaginationQueryHandlerTest()
        {
            var mapperConfig = new MapperConfiguration(c =>
            {
                c.AddProfile<MappingProfile>();
            });
            _mapper = mapperConfig.CreateMapper();

            var mockContext = new Mock<IApplicationDbContext>();
            IQueryable<WeatherForecastEntity> moqWeatherForecasts = new List<WeatherForecastEntity>
            {
                new WeatherForecastEntity {Id = 1, Date= DateTime.Now, Location = "dhaka", Summary="cold"},
                new WeatherForecastEntity {Id = 2, Date= DateTime.Now, Location = "barisal", Summary="cold"},
                new WeatherForecastEntity {Id = 3, Date= DateTime.Now, Location = "faridpur", Summary="cold"},
                new WeatherForecastEntity {Id = 4, Date= DateTime.Now, Location = "khulna", Summary="cold"},
                new WeatherForecastEntity {Id = 5, Date= DateTime.Now, Location = "madaripur", Summary="cold"},
                new WeatherForecastEntity {Id = 6, Date= DateTime.Now, Location = "rajbari", Summary="cold"},
                new WeatherForecastEntity {Id = 7, Date= DateTime.Now, Location = "magura", Summary="cold"},
            }.AsQueryable();

            var mockDbSet = new Mock<DbSet<WeatherForecastEntity>>();
            mockDbSet.As<IAsyncEnumerable<WeatherForecastEntity>>()
               .Setup(x => x.GetAsyncEnumerator(default))
               .Returns(new TestAsyncEnumerator<WeatherForecastEntity>(moqWeatherForecasts.GetEnumerator()));

            mockDbSet.As<IQueryable<WeatherForecastEntity>>()
                .Setup(m => m.Provider)
                .Returns(new TestAsyncQueryProvider<WeatherForecastEntity>(moqWeatherForecasts.Provider));

            mockDbSet.As<IQueryable<WeatherForecastEntity>>()
                .Setup(m => m.Expression)
                .Returns(moqWeatherForecasts.Expression);

            mockDbSet.As<IQueryable<WeatherForecastEntity>>()
                .Setup(m => m.ElementType)
                .Returns(moqWeatherForecasts.ElementType);

            mockDbSet.As<IQueryable<WeatherForecastEntity>>()
                .Setup(m => m.GetEnumerator())
                .Returns((IEnumerator<WeatherForecastEntity>)moqWeatherForecasts.GetEnumerator());

            mockContext.Setup(c => c.WeatherForecasts).Returns(mockDbSet.Object);
            _context = mockContext.Object;
        }

        [Fact]
        public async Task HandleTest() 
        {
            // Arrange
            var handler = new GetWeatherForecastWithPaginationQueryHandler(_context, _mapper);

            // Act
            var result = await handler.Handle(new GetWeatherForecastWithPaginationQuery(), CancellationToken.None);

            // Assert
            result.ShouldBeOfType<PaginatedList<WeatherForecastModel>>();
        }

    }
}