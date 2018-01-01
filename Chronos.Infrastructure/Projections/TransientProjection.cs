using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using Chronos.Infrastructure.Events;
using Chronos.Infrastructure.Interfaces;
using NodaTime;

namespace Chronos.Infrastructure.Projections
{
    public class TransientProjection<T> : Projection, ITransientProjection<T>
        where T : class, IReadModel, new()
    {
        internal TransientProjection(IEventStore eventStore)
            : base(eventStore) { }

        public T State { get; } = new T();

        protected override void When(StreamDetails stream, IList<IEvent> events)
        {
            foreach(var e in events)
                State.When(e);
            base.When(stream, events);
        }

        protected override int GetVersion(StreamDetails stream)
        {
            return State.Version;
        }
    }
}