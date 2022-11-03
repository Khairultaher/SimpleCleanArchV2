using Microsoft.EntityFrameworkCore.Migrations;
using WeatherForecast.Domain.Constants;
using WeatherForecast.Domain.Extensions;

#nullable disable

namespace WeatherForecast.Infrastucture.Migrations
{
    public partial class v1_3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RunSqlScript(View.LocationTemperatureSummery);
            migrationBuilder.RunSqlScript(Procedure.GetWeatherInformation);
            migrationBuilder.RunSqlScript(Function.GetTemperatureByLocation);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(string.Format(@"DROP VIEW IF EXISTS {0}", View.LocationTemperatureSummery));
            migrationBuilder.Sql(string.Format(@"DROP PROCEDURE IF EXISTS {0}", Procedure.GetWeatherInformation));
            migrationBuilder.Sql(string.Format(@"DROP FUNCTION IF EXISTS dbo.{0}", Function.GetTemperatureByLocation));
        }
    }
}
