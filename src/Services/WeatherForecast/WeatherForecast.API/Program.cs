using EventBus.Common;
using FluentValidation.AspNetCore;
using MicroElements.Swashbuckle.FluentValidation;
using MicroElements.Swashbuckle.FluentValidation.AspNetCore;
using Microsoft.OpenApi.Models;
using System.Reflection;
using WeatherForecast.API.Services;
using WeatherForecast.Application;
using WeatherForecast.Application.Constants;
using WeatherForecast.Application.Services;
using WeatherForecast.Infrastructure;
using WeatherForecast.Infrastructure.Middlewares;
using static WeatherForecast.Application.Constants.AppConstants;

var builder = WebApplication.CreateBuilder(args);

#region Configuration
builder.Configuration.AddJsonFile($"appsettings.json", false, true);
var env = builder.Configuration.GetSection("Environment").Value;
builder.Configuration.AddJsonFile($"appsettings.{env}.json", false, true);
//IConfiguration configuration = new ConfigurationBuilder().AddJsonFile($"appsettings.{env}.json").Build();                           .Build();
IConfiguration configuration = builder.Configuration;
#endregion

#region Constants & Variables
AppConstants.JwtSettings.Issuer = configuration["JwtSettings:Issuer"];
AppConstants.JwtSettings.Audience = configuration["JwtSettings:Audience"];
AppConstants.JwtSettings.SigningKey = configuration["JwtSettings:SigningKey"];
EventBusConstants.RabbitMQSettings.Host = configuration["RabbitMQSettings:Host"];
EventBusConstants.RabbitMQSettings.HostAddress = configuration["RabbitMQSettings:HostAddress"];
AppConstants.ServiceSettings.ServiceName = configuration["ServiceSettings:ServiceName"];
#endregion


// Add services to the container.
builder.Services.AddApplication();
builder.Services.AddInfrastructure(configuration);
builder.Services.AddSingleton<ICurrentUserService, CurrentUserService>();
builder.Services.AddTransient<ExceptionHandlingMiddleware>();


builder.Services.AddHttpContextAccessor();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddControllers().AddFluentValidation(c =>
{
    c.RegisterValidatorsFromAssembly(Assembly.GetExecutingAssembly());
    // Optionally set validator factory if you have problems with scope resolve inside validators.
    c.ValidatorFactoryType = typeof(HttpContextServiceProviderValidatorFactory);
});
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Clinical Trial Subject", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 1safsfsdfdfd\"",
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement {
        {
            new OpenApiSecurityScheme {
                Reference = new OpenApiReference {
                    Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});
builder.Services.AddFluentValidationRulesToSwagger();

//services cors
builder.Services.AddCors(p => p.AddPolicy("cors", builder =>
{
    builder.WithOrigins("*").AllowAnyMethod().AllowAnyHeader();
}));

var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    //app.UseSwagger(s => s.SerializeAsV2 = true);
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "CTSD V1");
        c.DefaultModelsExpandDepth(-1);

    });
}

// custom middleware
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseCors("cors");
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    //    endpoints.MapGet("/", async context =>
    //    {
    //        await context.Response.WriteAsync("Yes, I am on...");
    //    });
});

app.Run();
