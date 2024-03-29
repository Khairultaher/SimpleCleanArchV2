﻿
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using ValidationException = WeatherForecast.Application.Exceptions.ValidationException;

namespace WeatherForecast.Application.Behaviours;

//public class ValidationBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
//    where TRequest : IRequest<TResponse>
//  {
//      private readonly IEnumerable<IValidator<TRequest>> _validators;

//      public ValidationBehaviour(IEnumerable<IValidator<TRequest>> validators)
//      {
//          _validators = validators;
//      }

//      public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
//      {
//          if (_validators.Any())
//          {
//              var context = new ValidationContext<TRequest>(request);

//              var validationResults = await Task.WhenAll(_validators.Select(v => v.ValidateAsync(context, cancellationToken)));
//              var failures = validationResults.SelectMany(r => r.Errors).Where(f => f != null).ToList();

//              if (failures.Count != 0)
//                  throw new ValidationException(failures);
//          }
//          return await next();
//      }
//  

public sealed class ValidationBehaviour<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse> where TRequest : class, IRequest<TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehaviour(IEnumerable<IValidator<TRequest>> validators) => _validators = validators;

    public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
    {
        if (!_validators.Any())
        {
            return await next();
        }

        var context = new ValidationContext<TRequest>(request);

        var errorsDictionary = _validators
            .Select(x => x.Validate(context))
            .SelectMany(x => x.Errors)
            .Where(x => x != null)
            .GroupBy(
                x => x.PropertyName,
                x => x.ErrorMessage,
                (propertyName, errorMessages) => new
                {
                    Key = propertyName,
                    Values = errorMessages.Distinct().ToArray()
                })
            .ToDictionary(x => x.Key, x => x.Values);

        if (errorsDictionary.Any())
        {
            throw new ValidationException(errorsDictionary);
        }

        return await next();
    }
}

