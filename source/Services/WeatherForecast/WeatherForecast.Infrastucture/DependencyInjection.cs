using EventBus.Common;
using EventBus.Events;
using MassTransit;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using RabbitMQ.Client;
using System.Text;
using WeatherForecast.Application.Constants;
using WeatherForecast.Application.Interfaces.Identity;
using WeatherForecast.Application.Interfaces.Persistence;
using WeatherForecast.Application.Interfaces.Security;
using WeatherForecast.Application.Services;
using WeatherForecast.Infrastructure.Identity;
using WeatherForecast.Infrastructure.Persistence;
using WeatherForecast.Infrastructure.Security;
using WeatherForecast.Infrastructure.Services;
using WeatherForecast.Infrastucture.Persistence;

namespace WeatherForecast.Infrastructure;


public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        if (configuration.GetValue<bool>("UseInMemoryDatabase"))
        {
            services.AddDbContext<ApplicationReadDbContext>(options =>
                options.UseInMemoryDatabase("CleanArchitectureDb"));
        }
        else
        {
            services.AddDbContext<ApplicationWriteDbContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("WriteConnection")!,
                    b => b.MigrationsAssembly(typeof(ApplicationWriteDbContext).Assembly.FullName)));

            services.AddDbContext<ApplicationReadDbContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("ReadConnection")!,
                    b => b.MigrationsAssembly(typeof(ApplicationReadDbContext).Assembly.FullName)));
        }

        services.AddScoped<IApplicationWriteDbContext>(provider => provider.GetRequiredService<ApplicationWriteDbContext>());
        services.AddScoped<IApplicationReadDbContext>(provider => provider.GetRequiredService<ApplicationReadDbContext>());

        services.AddScoped<IDomainEventService, DomainEventService>();

        //services
        //    .AddDefaultIdentity<ApplicationUser>()
        //    .AddRoles<ApplicationRole>()
        //    .AddEntityFrameworkStores<ApplicationWriteDbContext>();


        // add identity services
        services
            .AddIdentityApiEndpoints<ApplicationUser>()
            .AddRoles<ApplicationRole>()
            .AddEntityFrameworkStores<ApplicationWriteDbContext>();

        //services.AddIdentityServer()
        //    .AddApiAuthorization<ApplicationUser, ApplicationDbContext>();

        services.AddTransient<IIdentityService, IdentityService>();
        //services.AddAuthentication().AddIdentityServerJwt();

        services.AddTransient<IJwtTokenHelper, JwtTokenHelper>();
        //services.AddTransient<ICsvFileBuilder, CsvFileBuilder>();

        //hosted services
        services.AddHostedService<DatabaseSeedingService>();
        // obsolete 
        //services.AddTransient<ApplicationDbContextSeed>();

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

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultSignOutScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options =>
        {
            options.SaveToken = true;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = AppConstants.JwtSettings.Issuer,
                ValidateAudience = true,
                ValidAudience = AppConstants.JwtSettings.Audience,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(AppConstants.JwtSettings.SigningKey))
            };
        });

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

        #region MassTransit
        services.AddMassTransit(config =>
        {
            config.UsingRabbitMq((ctx, cfg) =>
            {
                cfg.Send<WeatherForecastEvent>(x =>
                {
                    // use customeType for the routing/binding key
                    x.UseRoutingKeyFormatter(context => context.Message.EventType?.ToString()); // route by provider (CREATE or UPDATE)

                    // multiple conventions can be set, in this case also CorrelationId
                    //x.UseCorrelationId(context => context.Message.TransactionId);
                });
                cfg.Message<WeatherForecastEvent>(x => x.SetEntityName(EventBusConstants.Exchages.WeatherForecastExchange));
                cfg.Publish<WeatherForecastEvent>(x => x.ExchangeType = ExchangeType.Direct);
            });
        });

        #region OPTIONAL
        // OPTIONAL, but can be used to configure the bus options
        //services.AddOptions<MassTransitHostOptions>()
        //    .Configure(options =>
        //    {
        //        // if specified, waits until the bus is started before
        //        // returning from IHostedService.StartAsync
        //        // default is false
        //        options.WaitUntilStarted = true;

        //        // if specified, limits the wait time when starting the bus
        //        options.StartTimeout = TimeSpan.FromSeconds(10);

        //        // if specified, limits the wait time when stopping the bus
        //        options.StopTimeout = TimeSpan.FromSeconds(30);
        //    });
        #endregion

        #endregion


        return services;
    }
}

