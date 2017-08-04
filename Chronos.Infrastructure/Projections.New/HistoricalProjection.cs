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

        internal HistoricalProjection(IEventStoreSubscriptions eventStore, Instant date)
            : base(eventStore)
        {
            _date = date;
        }
        internal HistoricalProjection(Projection<T> projection, Instant date)
            : base(projection)
        {
            _date = date;
        }

        protected override IObservable<IEvent> GetEvents(StreamDetails stream) => base.GetEvents(stream)
            .Where(e => e.Timestamp <= _date);
    }
}