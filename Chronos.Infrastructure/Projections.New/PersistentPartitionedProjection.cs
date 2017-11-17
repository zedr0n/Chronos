using System;
using System.Linq;
using System.Reactive.Linq;
using System.Reflection;
using Chronos.Infrastructure.Events;
using Chronos.Infrastructure.Interfaces;
using NodaTime;

namespace Chronos.Infrastructure.Projections.New
{
    public class PersistentPartitionedProjection<T> : PersistentProjection<Guid,T>
        where T : class, IReadModel, new()
    {
        public PersistentPartitionedProjection(IEventStore eventStore, IStateWriter writer, IReadRepository readRepository) 
            : base(eventStore, writer, readRepository)
        {
            KeyFunc = s => s.Key;
        }

        protected override void Reset(ref IObservable<GroupedObservable<StreamDetails, IEvent>> events)
        {
            events = events.Select(x => new GroupedObservable<StreamDetails, IEvent>
            {
                Key = x.Key,
                Observable = x.Observable.StartWith(ResetState())
            });
        }
    }
}