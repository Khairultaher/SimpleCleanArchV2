using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeatherForecast.Application.Constants
{
    public static class AppConstants
    {
        public static string BaseUrl = "";
        public static string DateFormat = "yyyy-MM-dd";
        public static string LongDateFormat = "yyyy-MM-dd hh:mm tt";
        public static string FiscalYearClaimName = "FY";
        public static string ConnectionString = "";
        public static string RedisConnection = "";

        public static class EmailSetup
        {
            public static string FromEmail { get; set; } = "";
            public static string FromName { get; set; } = "";
            public static string SMTP_USERNAME { get; set; } = "";
            public static string SMTP_PASSWORD { get; set; } = "";
            public static string SMTP_PORT { get; set; } = "";
            public static string SMTP_HOST { get; set; } = "";
            public static string FB_Key { get; set; } = "";
        }


        public static bool UseJwtToken = true;
        public static class JwtSettings
        {
            public static string Issuer = "localhost";
            public static string Audience = "localhost";
            public static string SigningKey = "localhostWith@strongKey";
            public static int TokenTimeoutMinutes = 5;
            public static int RefreshTokenExpiryMinutes = 60;
        }

        public static class ServiceSettings
        {
            public static string ServiceName = "WeatherForecast";
        }
    }
}
