using System;
using System.Collections.Generic;
using Chronos.Infrastructure.Events;
using NodaTime;

namespace Chronos.Infrastructure.Projections
{
    public class Projector<T> : IProjector
        where T : class,IProjection
    {
        private readonly IProjectionWriter<T> _writer;

        private readonly HashSet<Instant> _asOf = new HashSet<Instant> { Instant.MaxValue };

        protected Projector(IProjectionWriter<T> writer, IEventBus eventBus)
        {
            _writer = writer;
            this.RegisterAll(eventBus);
        }

        protected void UpdateProjection(IEvent e, Action<T> action )
        {
            _writer.UpdateOrThrow(e.SourceId, action, e.Timestamp);
        }

        protected void AddProjection(IEvent e, Func<T> projection )
        {
            foreach (var asOf in _asOf)
                _writer.Add(e.SourceId, projection(), asOf);
        }

        public void Rebuild()
        {
            /*var events = _eventStore.GetEvents()
                .OrderBy(e => e.Timestamp);

            foreach(var e in events)
                this.Dispatch(e);*/
        }

        public void Rebuild(Instant upTo)
        {
            /*_asOf.Add(upTo);
            Rebuild();*/
        }

    }
}