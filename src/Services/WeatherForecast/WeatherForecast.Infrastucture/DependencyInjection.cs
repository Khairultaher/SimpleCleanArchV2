using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using WeatherForecast.Application.Common.Constants;
using WeatherForecast.Application.Common.Interfaces;
using WeatherForecast.Infrastructure.Identity;
using WeatherForecast.Infrastructure.Middlewares;
using System.Text;
using WeatherForecast.Infrastructure.Persistence;
using WeatherForecast.Infrastructure.Services;
using System;
using WeatherForecast.Infrastructure.Security;

namespace WeatherForecast.Infrastructure;


public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        if (configuration.GetValue<bool>("UseInMemoryDatabase"))
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseInMemoryDatabase("CleanArchitectureDb"));
        }
        else
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection"),
                    b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));
        }

        services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());

        services.AddScoped<IDomainEventService, DomainEventService>();

        services
            .AddDefaultIdentity<ApplicationUser>()
            .AddRoles<ApplicationRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>();


        //services.AddIdentityServer()
        //    .AddApiAuthorization<ApplicationUser, ApplicationDbContext>();

        services.AddTransient<IIdentityService, IdentityService>();
        //services.AddAuthentication().AddIdentityServerJwt();

        services.AddTransient<IJwtTokenHelper, JwtTokenHelper>();
        //services.AddTransient<ICsvFileBuilder, CsvFileBuilder>();



        //hosted services
        services.AddHostedService<DatabaseSeedingService>();
        services.AddTransient<ApplicationDbContextSeed>();

        #region Configure Session
        services.AddSession(options =>
        {
            options.Cookie.HttpOnly = true;
            options.IdleTimeout = TimeSpan.FromHours(1);
        });
        #endregion

        #region Configure Token & Cookie Based Authentication Together

        //builder.Services.AddAuthentication(config =>
        //{
        //    config.DefaultScheme = "AppCookies";

        //}).AddPolicyScheme("AppCookies", "Cookies or JWT", options =>
        //{
        //    options.ForwardDefaultSelector = context =>
        //    {
        //        var bearerAuth = context.Request.Headers["Authorization"].FirstOrDefault()?.StartsWith("Bearer ") ?? false;
        //        if (bearerAuth)
        //            return JwtBearerDefaults.AuthenticationScheme;
        //        else
        //            return CookieAuthenticationDefaults.AuthenticationScheme;
        //    };
        //}).AddCookie(options =>
        //{
        //    options.Cookie.Name = "AppCookies";
        //    options.LoginPath = new PathString("/auth/login");
        //    options.AccessDeniedPath = "/Auth/Login"; ;
        //    options.LogoutPath = new PathString("/auth/logout");
        //    options.SlidingExpiration = true;
        //    options.ExpireTimeSpan = TimeSpan.FromHours(1);

        //}).AddJwtBearer(options =>
        //{
        //    options.SaveToken = true;
        //    options.TokenValidationParameters = new TokenValidationParameters
        //    {
        //        ValidateIssuer = true,
        //        ValidIssuer = Constants.JwtToken.Issuer,
        //        ValidateAudience = true,
        //        ValidAudience = Constants.JwtToken.Audience,
        //        ValidateIssuerSigningKey = true,
        //        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Constants.JwtToken.SigningKey))
        //    };
        //});
        #endregion

        #region Configure Cookie Based Authentication
        services.AddAuthentication("AppCookies")
            .AddCookie("AppCookies", options =>
            {
                options.Cookie.Name = "AppCookies";
                //options.LoginPath = new PathString("/auth/login");
                //options.AccessDeniedPath = new PathString("/auth/login");
                //options.LogoutPath = new PathString("/auth/logout");
                options.SlidingExpiration = true;
                options.ExpireTimeSpan = TimeSpan.FromHours(1);
                options.Events = new CookieAuthenticationEvents()
                {

                };
            });
        #endregion

        #region Configure Token Based Authentication 

        //services.AddAuthentication(options =>
        //{
        //    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        //    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        //    options.DefaultSignOutScheme = JwtBearerDefaults.AuthenticationScheme;
        //}).AddJwtBearer(options =>
        //{
        //    options.SaveToken = true;
        //    options.TokenValidationParameters = new TokenValidationParameters
        //    {
        //        ValidateIssuer = true,
        //        ValidIssuer = Constants.JwtSettings.Issuer,
        //        ValidateAudience = true,
        //        ValidAudience = Constants.JwtSettings.Audience,
        //        ValidateIssuerSigningKey = true,
        //        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Constants.JwtSettings.SigningKey))
        //    };
        //});

        #endregion

        #region Configure Authorization with Policy
        services.AddAuthorization(options =>
        {
            options.AddPolicy("Admin", policy => policy
                    .RequireRole("Admin")
                    .RequireClaim("Department", "IT"));

            options.AddPolicy("AccountsAdmin", policy => policy.RequireClaim("Depertment", "Accounts")
                                                     .RequireRole("Admin"));

            options.AddPolicy("SysAdmin", policy => policy.RequireRole("SysAdmin"));

            options.AddPolicy("HRManagerOnly", policy => policy
                    .RequireClaim("Department", "HR")
                    .RequireClaim("Manager")
                    .Requirements.Add(new HRManagerProbationRequirement(3)));
        });

        //services.AddHttpClient("OurWebAPI", client =>
        //{
        //    client.BaseAddress = new Uri("https://localhost:44336/");
        //var httpClient = httpClientFactory.CreateClient("OurWebAPI");
        //});

        #endregion

        //services.AddTransient<ExceptionHandlingMiddleware>();

        return services;
    }
}

