using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WeatherForecast.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeatherForecast.Infrastructure.Persistence.Configurations;

public class WeatherForecastConfiguration : IEntityTypeConfiguration<WeatherForecastEntity>
{
    public void Configure(EntityTypeBuilder<WeatherForecastEntity> builder)
    {
        builder.ToTable("WeatherForecasts", "dbo", tb => tb.IsTemporal(ttb =>
        {
            ttb.HasPeriodStart("ValidFrom");
            ttb.HasPeriodEnd("ValidTo");
            ttb.UseHistoryTable("WeatherForecastsHistory", "dbo");
        })).HasKey(x => x.Id);
        //builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();

        builder.Property(x => x.TemperatureC).IsRequired(true);
        builder.Ignore(e => e.TemperatureF);

        builder.Property(t => t.Summary)
            .HasMaxLength(250)
            .IsRequired();
    }
}
