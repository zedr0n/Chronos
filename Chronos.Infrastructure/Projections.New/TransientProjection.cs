using System;
using System.Reactive.Linq;
using Chronos.Infrastructure.Events;
using Chronos.Infrastructure.Interfaces;
using NodaTime;

namespace Chronos.Infrastructure.Projections.New
{
    public class TransientProjection<T> : Projection, ITransientProjection<T>
        where T : class, IReadModel, new()
    {
        internal TransientProjection(IEventStore eventStore)
            : base(eventStore) { }

        public T State { get; } = new T();

        protected override void When(StreamDetails stream, IEvent e)
        {
            State.When(e);
            base.When(stream, e);
        }

        protected override void Reset(ref IObservable<GroupedObservable<StreamDetails, IEvent>> events)
        {
            events = events.StartWith(new GroupedObservable<StreamDetails, IEvent>
            {
                Key = new StreamDetails("Dummy"),
                Observable = Observable.Return(ResetState())
            });
        }

        protected override int GetVersion(StreamDetails stream)
        {
            return State.Version;
        }
    }
}