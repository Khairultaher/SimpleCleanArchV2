using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeatherForecast.Application.WeatherForecasts.Commands.Update
{
    public class UpdateWeatherForecastCommandValidator : AbstractValidator<UpdateWeatherForecastCommand>
    {
        public UpdateWeatherForecastCommandValidator()
        {
            RuleFor(v => v.Summary)
                 .MaximumLength(200)
                 .NotEmpty();

            RuleFor(v => v.TemperatureC)
                .NotNull().WithMessage("Title is required.")
                .GreaterThanOrEqualTo(1).WithMessage("Temperature(C) at least greater than or equal to 1.");
        }
    }
}
