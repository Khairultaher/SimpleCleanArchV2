using WeatherForecast.API.Services;
using WeatherForecast.Application;
using WeatherForecast.Application.Common;
using WeatherForecast.Application.Common.Constants;
using WeatherForecast.Infrastructure;
using WeatherForecast.Infrastructure.Middlewares;

var builder = WebApplication.CreateBuilder(args);

#region Configuration
builder.Configuration.AddJsonFile($"appsettings.json", false, true);
var env = builder.Configuration.GetSection("Environment").Value;
builder.Configuration.AddJsonFile($"appsettings.{env}.json", false, true);
//IConfiguration configuration = new ConfigurationBuilder().AddJsonFile($"appsettings.{env}.json").Build();                           .Build();
IConfiguration configuration = builder.Configuration;
#endregion

#region Constants & Variables
Constants.JwtSettings.Issuer = configuration["JwtSettings:Issuer"];
Constants.JwtSettings.Audience = configuration["JwtSettings:Audience"];
Constants.JwtSettings.SigningKey = configuration["JwtSettings:SigningKey"];
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
