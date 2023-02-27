using Microsoft.EntityFrameworkCore;
using WeatherForecast.Application.Interfaces.Persistence;
using WeatherForecast.Domain.Entities;
using WeatherForecast.Infrastructure.Persistence;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace WeatherForecast.Infrastructure.Repositories
{
    public class WeatherForecastRepository : RepositoryBase<WeatherForecastEntity>, IWeatherForecastRepository
    {
        public WeatherForecastRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<IEnumerable<WeatherForecastEntity>> GetWeatherForecastByUserName(string userName)
        {
            var orderList = await _dbContext.WeatherForecasts
                                .Where(o => o.CreatedBy == userName)
                                .ToListAsync();
            return orderList;
        }
    }
}
