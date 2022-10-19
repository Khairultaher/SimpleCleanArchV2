using MediatR;
using Microsoft.Extensions.Logging;
using WeatherForecast.Application.Models;
using WeatherForecast.Domain.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using WeatherForecast.Domain.Entities;

namespace WeatherForecast.Application.Features.WeatherForecast.EventHandlers
{
    public class WeatherForecastCreatedEventHandler : INotificationHandler<DomainEventNotification<EntityCreatedEvent<WeatherForecastEntity>>>
    {
        private readonly ILogger<WeatherForecastCreatedEventHandler> _logger;

        public WeatherForecastCreatedEventHandler(ILogger<WeatherForecastCreatedEventHandler> logger)
        {
            _logger = logger;
        }

        public Task Handle(DomainEventNotification<EntityCreatedEvent<WeatherForecastEntity>> notification, CancellationToken cancellationToken)
        {
            var domainEvent = notification.DomainEvent;

            _logger.LogInformation("CleanArchitecture Domain Event: {DomainEvent}", domainEvent.GetType().Name);

            return Task.CompletedTask;
        }
    }
}
