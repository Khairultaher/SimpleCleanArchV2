using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;
using WeatherForecast.Domain.Dtos;
using WeatherForecast.Domain.Entities;

namespace WeatherForecast.Application.Interfaces.Persistence;

public interface IApplicationDbContext
{
    #region TABLES
    DbSet<WeatherForecastEntity> WeatherForecasts { get; }
    #endregion
    #region VIEWS
    /// <summary>
    /// vwLocationTemperatureSummery
    /// </summary>
    DbSet<LocationTemperatureSummeryDto> LocationTemperatureSummery { get; }
    #endregion

    #region PROCEDURES
    /// <summary>
    /// spGetWeatherInformation()
    /// </summary>
    DbSet<WeatherInformationDto> GetWeatherInformation { get; }
    #endregion

    #region FUNCTIONS
    /// <summary>
    /// spGetTemperatureByLocation(@location nvarchar(50))
    /// </summary>
    DbSet<TemperatureByLocationDto> GetTemperatureByLocation { get; }
    #endregion
}
public interface ISaveChange
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}


public interface IApplicationWriteDbContext: IApplicationDbContext, ISaveChange
{ 
}

public interface IApplicationReadDbContext : IApplicationDbContext
{
}