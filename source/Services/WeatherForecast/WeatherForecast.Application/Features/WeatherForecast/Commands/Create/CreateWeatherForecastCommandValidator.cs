using FluentValidation;

namespace WeatherForecast.Application.Features.WeatherForecast.Commands.Create
{
    public class CreateWeatherForecastCommandValidator : AbstractValidator<CreateWeatherForecastCommand>
    {
        public CreateWeatherForecastCommandValidator()
        {
            RuleFor(v => v.Summary)
                .MaximumLength(200)
                .NotEmpty().WithMessage("Summary is required.");

            RuleFor(v => v.TemperatureC)
                .NotNull().WithMessage("TemperatureC is required.")
                .GreaterThanOrEqualTo(1).WithMessage("Temperature(C) at least greater than or equal to 1.");
        }
    }
}
