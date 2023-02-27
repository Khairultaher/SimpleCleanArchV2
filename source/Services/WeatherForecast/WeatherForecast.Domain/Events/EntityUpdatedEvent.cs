
using WeatherForecast.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeatherForecast.Domain.Events
{

    public class EntityUpdatedEvent<T> : DomainEvent where T: class
    {
        public EntityUpdatedEvent(T item)
        {
            Item = item;
        }
        public T Item { get; }
    }
}
