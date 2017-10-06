using System;
using System.Reactive.Linq;
using Chronos.Infrastructure.Interfaces;
using NodaTime;

namespace Chronos.Infrastructure.Projections.New
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

        protected override void When(StreamDetails stream, IEvent e)
        {
            if (e.Timestamp > _date)
                return;
            base.When(stream, e);
        }
    }
}