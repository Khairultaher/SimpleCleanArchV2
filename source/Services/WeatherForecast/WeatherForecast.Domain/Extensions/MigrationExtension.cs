using Microsoft.EntityFrameworkCore.Migrations;
using System.Reflection;
#nullable disable

namespace WeatherForecast.Domain.Extensions
{
    public static class NonTableObjectMigrationExtension
    {
        public static void RunNonTableSqlScript(this MigrationBuilder migrationBuilder, string script)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = assembly.GetManifestResourceNames().FirstOrDefault(x => x.EndsWith($"{script}.sql"));
            using var stream = assembly.GetManifestResourceStream(resourceName);
            using var reader = new StreamReader(stream);
            var sqlResult = reader.ReadToEnd();
            migrationBuilder.Sql(sqlResult);
        }
    }
}
