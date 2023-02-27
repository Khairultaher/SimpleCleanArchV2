using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WeatherForecast.Domain.Entities;
using WeatherForecast.Infrastructure.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace WeatherForecast.Infrastructure.Persistence;

public class DatabaseSeedingService : IHostedService
{
    private readonly IServiceProvider _serviceProvider;


    public DatabaseSeedingService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        context.Database.Migrate();

        var userManager = scope.ServiceProvider.GetService<UserManager<ApplicationUser>>();
        var roleManager = scope.ServiceProvider.GetService<RoleManager<ApplicationRole>>();

        await SeedInitialData(context, userManager, roleManager, cancellationToken);
    }

    private async Task SeedInitialData(ApplicationDbContext context
        , UserManager<ApplicationUser> userManager
        , RoleManager<ApplicationRole> roleManager
        , CancellationToken cancellationToken)
    {
        await SeedDefaultUserAsync(userManager, roleManager);
        await SeedSampleDataAsync(context);
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    #region Internal functions
    public async Task SeedDefaultUserAsync(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
    {
        var roles = new List<ApplicationRole>()
        {
            new ApplicationRole(Guid.NewGuid().ToString(), "SysAdmin", "System Admin"),
            new ApplicationRole(Guid.NewGuid().ToString(), "Admin", "Admin"),
            new ApplicationRole(Guid.NewGuid().ToString(), "User", "General user")
        };

        if (!roleManager.Roles.Any())
        {
            foreach (var role in roles)
            {
                await roleManager.CreateAsync(role);
            }

        }

        var administrator = new ApplicationUser { Id = Guid.NewGuid().ToString(), UserName = "admin", Email = "admin@localhost" };

        var claims = new List<Claim> {
            new Claim("Department", "IT"),
            new Claim("Designation", "Software Engineer")
        };
        if (userManager.Users.All(u => u.UserName != administrator.UserName))
        {
            var res = await userManager.CreateAsync(administrator, "Qwe@1234");
            await userManager.AddToRolesAsync(administrator, roles.Select(s => s.Name).ToList());
            await userManager.AddClaimsAsync(administrator, claims);
        }
    }

    public async Task SeedSampleDataAsync(ApplicationDbContext context)
    {
        // Seed, if necessary
        if (!context.WeatherForecasts.Any())
        {
            string[] Summaries = new[]
            {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
            };
            string[] locations = new[]
            {
            "Dhaka", "Faridpur", "Rajbari", "Jashor", "Khulna", "Potuakhali", "Munsigang"
            };
            var data = Enumerable.Range(1, 100).Select(index => new WeatherForecastEntity
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = Random.Shared.Next(25, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)],
                Location = locations[Random.Shared.Next(locations.Length)]
            })
            .ToArray();
            await context.WeatherForecasts.AddRangeAsync(data);

            await context.SaveChangesAsync();
        }
    }
    #endregion
}



