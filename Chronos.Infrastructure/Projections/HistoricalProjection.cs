using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using Chronos.Infrastructure.Interfaces;
using NodaTime;

namespace Chronos.Infrastructure.Projections
{
    public class HistoricalProjection<T> : TransientProjection<T>
        where T : class, IReadModel, new()
    {
        private readonly Instant _date;

        internal HistoricalProjection(IEventStore eventStore,Instant date)
            : base(eventStore)
        {
            _date = date;
        }

        protected override void When(StreamDetails stream, IList<IEvent> events)
        {
            //if (e.Timestamp > _date)
            //    return;
            
            base.When(stream, events.Where(e => e.Timestamp <= _date).ToList());
        }
    }
}