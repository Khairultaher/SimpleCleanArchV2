using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using WeatherForecast.API.ViewModels;
using WeatherForecast.Application.Features.WeatherForecast.Commands.Create;
using WeatherForecast.Application.Features.WeatherForecast.Commands.Update;
using WeatherForecast.Application.Features.WeatherForecast.Queries.GetWeatherForecast;

namespace WeatherForecast.API.ExampleModels
{
    public class ExampleSchemaFilter : ISchemaFilter
    {
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            if (context.Type == typeof(LoginModel))
            {
                schema.Example = new OpenApiObject()
                {
                    ["UserName"] = new OpenApiString("admin"),
                    ["PassWord"] = new OpenApiString("123")
                };
            }
            else if (context.Type == typeof(GetWeatherForecastWithPaginationQuery))
            {
                schema.Example = new OpenApiObject()
                {
                    ["PageNumber"] = new OpenApiString("1"),
                    ["PageSize"] = new OpenApiString("10"),
                };
            }
            else if (context.Type == typeof(CreateWeatherForecastCommand))
            {
                schema.Example = new OpenApiObject()
                {
                    ["TemperatureC"] = new OpenApiString("30"),
                    ["Location"] = new OpenApiString("Faridpur"),
                    ["Summary"] = new OpenApiString("Hot"),
                };
            }
            else if (context.Type == typeof(UpdateWeatherForecastCommand))
            {
                schema.Example = new OpenApiObject()
                {
                    ["Id"] = new OpenApiString("10"),
                    ["TemperatureC"] = new OpenApiString("30"),
                    ["Location"] = new OpenApiString("Faridpur"),
                    ["Summary"] = new OpenApiString("Hot"),
                };
            }

        }
    }
}
