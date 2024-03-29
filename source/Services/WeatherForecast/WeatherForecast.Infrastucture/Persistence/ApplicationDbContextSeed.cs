﻿using Microsoft.AspNetCore.Identity;
using WeatherForecast.Domain.Entities;
using WeatherForecast.Infrastructure.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeatherForecast.Infrastructure.Persistence;

public class ApplicationDbContextSeed
{
    public async Task SeedDefaultUserAsync(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        var administratorRole = new IdentityRole("Admin");

        if (roleManager.Roles.All(r => r.Name != administratorRole.Name))
        {
            await roleManager.CreateAsync(administratorRole);
        }

        var administrator = new ApplicationUser { UserName = "admin@localhost", Email = "admin@localhost" };

        if (userManager.Users.All(u => u.UserName != administrator.UserName))
        {
            await userManager.CreateAsync(administrator, "admin!");
            await userManager.AddToRolesAsync(administrator, new[] { administratorRole.Name });
        }
    }

    public async Task SeedSampleDataAsync(ApplicationWriteDbContext context)
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
                Summary = locations[Random.Shared.Next(Summaries.Length)],
                Location = locations[Random.Shared.Next(locations.Length)]
            })
            .ToArray();
            await context.WeatherForecasts.AddRangeAsync(data);

            await context.SaveChangesAsync();
        }
    }
}
