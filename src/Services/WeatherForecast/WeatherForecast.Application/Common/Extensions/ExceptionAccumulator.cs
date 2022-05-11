using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeatherForecast.Application.Common.Extensions
{
    public static class ExceptionAccumulator
    {
        public static string GetExceptions(this Exception ex)
        {
            var messages = new List<string>();
            do
            {
                messages.Add(ex.Message);
                ex = ex.InnerException;
            }
            while (ex != null);

            return string.Join("", messages);
        }

        public static List<string> GetExceptionList(this Exception ex)
        {
            var messages = new List<string>();
            do
            {
                messages.Add(ex.Message);
                ex = ex.InnerException;
            }
            while (ex != null);

            return messages;
        }
    }
}
