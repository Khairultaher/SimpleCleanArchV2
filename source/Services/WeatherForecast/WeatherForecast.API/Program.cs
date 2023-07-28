using EventBus.Common;
using FluentValidation.AspNetCore;
using MicroElements.Swashbuckle.FluentValidation;
using MicroElements.Swashbuckle.FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using Swashbuckle.AspNetCore.SwaggerUI;
using System.Reflection;
using WeatherForecast.API.Services;
using WeatherForecast.Application;
using WeatherForecast.Application.Constants;
using WeatherForecast.Application.Services;
using WeatherForecast.Infrastructure;
using WeatherForecast.Infrastructure.Middlewares;
#nullable disable

var builder = WebApplication.CreateBuilder(args);

#region Configuration
builder.Configuration.AddJsonFile($"appsettings.json", false, true).AddEnvironmentVariables();
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
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Weather Forecast",
        Version = "v1",
        TermsOfService = new Uri("https://github.com/Khairultaher"),
        Contact = new OpenApiContact
        {
            Name = "Support",
            Email = "support@abc.com",
            Url = new Uri("https://github.com/Khairultaher"),
        },
        License = new OpenApiLicense
        {
            Name = "Use under LICX",
            Url = new Uri("https://github.com/Khairultaher"),
        }
    });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        In = ParameterLocation.Header,
        // For JWT Bearer
        Name = "Authorization",
        Scheme = "Bearer",
        BearerFormat = "JWT",
        Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 1safsfsdfdfd\"",
        Type = SecuritySchemeType.Http,

        // For ApiKey Auth
        //Name = "ApiKey",
        //Scheme = "ApiKeyScheme",
        //Description = "ApiKey must appear in header",
        //Type = SecuritySchemeType.ApiKey,    
    });

    var securityRequirement = new OpenApiSecurityRequirement {
        {
            new OpenApiSecurityScheme {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    // For JWT Bearer
                    Id = "Bearer"

                    //For ApiKey Auth
                    //Id = "ApiKey"
                },
                In = ParameterLocation.Header
            },
            new string[] {}
        }
    };

    c.AddSecurityRequirement(securityRequirement);
    c.ExampleFilters();

    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);

    // group controllers
    c.TagActionsBy(api =>
    {
        if (api.GroupName != null)
        {
            if (api.ActionDescriptor is ControllerActionDescriptor controllerActionDes)
            {
                if (controllerActionDes.ControllerName == "Demographic")
                {
                    return new[] { api.GroupName + ": " + controllerActionDes.ControllerName + "/Subject" };
                }
                return new[] { api.GroupName + ": " + controllerActionDes.ControllerName };
            }
            //return new[] { api.GroupName };
        }

        if (api.ActionDescriptor is ControllerActionDescriptor controllerActionDescriptor)
        {
            if (controllerActionDescriptor.ControllerName == "Demographic")
            {
                return new[] { controllerActionDescriptor.ControllerName + "/Subject" };
            }
            return new[] { controllerActionDescriptor.ControllerName };
        }

        throw new InvalidOperationException("Unable to determine tag for endpoint.");
    });

    c.DocInclusionPredicate((name, api) => true);
});
builder.Services.AddFluentValidationRulesToSwagger();
builder.Services.AddSwaggerExamplesFromAssemblies(Assembly.GetEntryAssembly());

//services cors
builder.Services.AddCors(p => p.AddPolicy("cors", builder =>
{
    builder.WithOrigins("*").AllowAnyMethod().AllowAnyHeader();
}));


builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = "localhost:6379";
    // options.InstanceName = "WeatherForecastApp";
});


var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    //app.UseSwagger(s => s.SerializeAsV2 = true);
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.DocumentTitle = "WF API";
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "WF V1");
        c.DefaultModelsExpandDepth(-1);
        c.DocExpansion(DocExpansion.None);
    });
}

// custom middleware
app.UseMiddleware<ExceptionHandlingMiddleware>();
//app.UseMiddleware<ApiKeyAuthMiddleware>(); // when use api key
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
    endpoints.MapGet("/", async context =>
    {
        await context.Response.WriteAsync("Yes, I am on...");
    });
});

app.Run();
