using FluentValidation.Results;
using ApplicationException = WeatherForecast.Domain.Exceptions.ApplicationException;

namespace WeatherForecast.Application.Exceptions
{
    public sealed class ValidationException : ApplicationException
    {
        public ValidationException(IReadOnlyDictionary<string, string[]> errorsDictionary)
            : base("Validation Failure", "One or more validation errors occurred")
            => ErrorsDictionary = errorsDictionary;

        public IReadOnlyDictionary<string, string[]> ErrorsDictionary { get; }
    }

    //public sealed class ValidationException : Exception
    //{
    //    public ValidationException()
    //        : base("One or more validation failures have occurred.")
    //    {
    //        Errors = new Dictionary<string, string[]>();
    //    }

    //    public ValidationException(IEnumerable<ValidationFailure> failures)
    //        : this()
    //    {
    //        Errors = failures
    //            .GroupBy(e => e.PropertyName, e => e.ErrorMessage)
    //            .ToDictionary(failureGroup => failureGroup.Key, failureGroup => failureGroup.ToArray());
    //    }

    //    public IDictionary<string, string[]> Errors { get; }
    //}

}
