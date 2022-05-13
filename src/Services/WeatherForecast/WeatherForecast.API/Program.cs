using EventBus.Common;
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

//builder.Services.AddControllers(options =>
//    options.Filters.Add<ApiExceptionFilterAttribute>())
//        .AddFluentValidation(x => x.RegisterValidatorsFromAssembly(Assembly.GetExecutingAssembly()));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//services cors
builder.Services.AddCors(p => p.AddPolicy("cors", builder =>
{
    builder.WithOrigins("*").AllowAnyMethod().AllowAnyHeader();
}));

var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
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

//app.UseEndpoints(endpoints =>
//{
//    endpoints.MapGet("/", async context =>
//    {
//        await context.Response.WriteAsync("Yes, I am on...");
//    });
//});

app.Run();
