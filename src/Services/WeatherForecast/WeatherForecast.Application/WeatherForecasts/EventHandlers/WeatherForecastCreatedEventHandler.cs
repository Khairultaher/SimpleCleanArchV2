using MediatR;
using Microsoft.Extensions.Logging;
using WeatherForecast.Application.Common.Models;
using WeatherForecast.Domain.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace WeatherForecast.Application.WeatherForecasts.EventHandlers
{
    public class WeatherForecastCreatedEventHandler : INotificationHandler<DomainEventNotification<WeatherForecastCreatedEvent>>
    {
        private readonly ILogger<WeatherForecastCreatedEventHandler> _logger;

        public WeatherForecastCreatedEventHandler(ILogger<WeatherForecastCreatedEventHandler> logger)
        {
            _logger = logger;
        }

        public Task Handle(DomainEventNotification<WeatherForecastCreatedEvent> notification, CancellationToken cancellationToken)
        {
            var domainEvent = notification.DomainEvent;

            _logger.LogInformation("CleanArchitecture Domain Event: {DomainEvent}", domainEvent.GetType().Name);

            return Task.CompletedTask;
        }
    }
}
