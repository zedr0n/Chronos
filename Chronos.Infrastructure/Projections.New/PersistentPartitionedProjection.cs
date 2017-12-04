using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reflection;
using Chronos.Infrastructure.Events;
using Chronos.Infrastructure.Interfaces;
using NodaTime;

namespace Chronos.Infrastructure.Projections.New
{
    public abstract class PersistentPartitionedProjection<T> : PersistentProjection<Guid,T>
        where T : class, IReadModel, new()
    {
        public PersistentPartitionedProjection(IEventStore eventStore, IStateWriter writer, IReadRepository readRepository) 
            : base(eventStore, writer, readRepository)
        {
            Key = new KeySelector(s => s.Key);
        }

        protected virtual bool AddReset(StreamDetails stream) => true;
        
        protected override void Reset(ref IObservable<GroupedObservable<StreamDetails, IList<IEvent>>> events)
        {
            var completed = false;
            events = events.Select(x => new GroupedObservable<StreamDetails, IList<IEvent>>
            {
                Key = x.Key,
                Observable = !completed ? x.Observable.StartWith(new List<IEvent> { ResetState() }) : x.Observable
            }).Do(x => completed = true);
        }
    }
}