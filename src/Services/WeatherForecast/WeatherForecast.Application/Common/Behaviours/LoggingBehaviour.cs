using MediatR;
using MediatR.Pipeline;
using Microsoft.Extensions.Logging;
using WeatherForecast.Application.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WeatherForecast.Application.Common.Behaviours;


//public class LoggingBehaviour<TRequest> : IRequestPreProcessor<TRequest> where TRequest : notnull
//{
//    private readonly ILogger _logger;
//    private readonly ICurrentUserService _currentUserService;
//    private readonly IIdentityService _identityService;

//    public LoggingBehaviour(ILogger<TRequest> logger, ICurrentUserService currentUserService, IIdentityService identityService)
//    {
//        _logger = logger;
//        _currentUserService = currentUserService;
//        _identityService = identityService;
//    }

//    public async Task Process(TRequest request, CancellationToken cancellationToken)
//    {
//        var requestName = typeof(TRequest).Name;
//        var userId = _currentUserService.UserId ?? string.Empty;
//        string userName = string.Empty;

//        if (!string.IsNullOrEmpty(userId))
//        {
//            userName = await _identityService.GetUserNameAsync(userId);
//        }

//        _logger.LogInformation("CleanArchitecture Request: {Name} {@UserId} {@UserName} {@Request}",
//            requestName, userId, userName, request);
//    }
//}


public class LoggingBehaviour<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
{
    private readonly ILogger<LoggingBehaviour<TRequest, TResponse>> _logger;
    public LoggingBehaviour(ILogger<LoggingBehaviour<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }
    public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
    {
        //Request
        _logger.LogInformation($"Handling {typeof(TRequest).Name}");
        Type myType = request.GetType();
        IList<PropertyInfo> props = new List<PropertyInfo>(myType.GetProperties());
        foreach (PropertyInfo prop in props)
        {
            object propValue = prop.GetValue(request, null);
            _logger.LogInformation("{Property} : {@Value}", prop.Name, propValue);
        }
        var response = await next();
        //Response
        _logger.LogInformation($"Handled {typeof(TResponse).Name}");
        return response;
    }
}

