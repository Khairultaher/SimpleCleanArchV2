﻿using WeatherForecast.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeatherForecast.Application.Services;

public interface IDomainEventService
{
    Task Publish(DomainEvent domainEvent);
}