using Duende.IdentityServer.EntityFramework.Options;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Reflection;
using WeatherForecast.Application.Interfaces.Persistence;
using WeatherForecast.Application.Services;
using WeatherForecast.Domain.Common;
using WeatherForecast.Domain.Constants;
using WeatherForecast.Domain.Dtos;
using WeatherForecast.Domain.Entities;
using WeatherForecast.Infrastructure.Identity;

namespace WeatherForecast.Infrastructure.Persistence;

public class ApplicationReadDbContext
    //: AuthorizationDbContext<ApplicationUser, ApplicationRole, string>
    : IdentityDbContext<ApplicationUser, ApplicationRole, string>
    , IApplicationReadDbContext
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IDomainEventService _domainEventService;

    public ApplicationReadDbContext(
        DbContextOptions<ApplicationReadDbContext> options,
        IOptions<OperationalStoreOptions> operationalStoreOptions,
        ICurrentUserService currentUserService,
        IDomainEventService domainEventService) : base(options)
    {
        _currentUserService = currentUserService;
        _domainEventService = domainEventService;
    }

    #region TABLES
    public DbSet<WeatherForecastEntity> WeatherForecasts => Set<WeatherForecastEntity>();
    #endregion

    #region VIEWS
    /// <summary>
    /// vwLocationTemperatureSummery
    /// </summary>
    //public DbSet<LocationTemperatureSummeryDto> LocationTemperatureSummery => Set<LocationTemperatureSummeryDto>();
    #endregion

    #region PROCEDURES
    /// <summary>
    /// spGetWeatherInformation(@location nvarchar(50)
    /// </summary>
    //public DbSet<WeatherInformationDto> GetWeatherInformation => Set<WeatherInformationDto>();
    #endregion

    #region FUNCTIONS
    /// <summary>
    /// fnGetTemperatureByLocation(@location nvarchar(50)
    /// </summary>
    //public DbSet<TemperatureByLocationDto> GetTemperatureByLocation => Set<TemperatureByLocationDto>();
    #endregion

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(builder);

        #region TABLES

        #endregion

        #region VIEWS
        //builder.Entity<LocationTemperatureSummeryDto>(x =>
        //{
        //    x.HasNoKey();
        //    x.ToView(View.LocationTemperatureSummery);
        //    x.Metadata.SetIsTableExcludedFromMigrations(true);
        //});
        #endregion

        #region FUNCTIONS
        //builder.Entity<TemperatureByLocationDto>(x =>
        //{
        //    x.HasNoKey();
        //    x.ToFunction(Function.GetTemperatureByLocation);
        //    x.Metadata.SetIsTableExcludedFromMigrations(true);
        //});
        #endregion

        #region PROCEDURES
        //builder.Entity<WeatherInformationDto>(x =>
        //{
        //    x.HasNoKey();
        //    x.ToView(Procedure.GetWeatherInformation);
        //    x.Metadata.SetIsTableExcludedFromMigrations(true);
        //});
        #endregion
    }

    private async Task DispatchEvents(DomainEvent[] events)
    {
        foreach (var @event in events)
        {
            @event.IsPublished = true;
            await _domainEventService.Publish(@event);
        }
    }
}
