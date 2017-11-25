using System;
using System.Reactive.Linq;
using Chronos.Infrastructure.Events;
using Chronos.Infrastructure.Interfaces;

namespace Chronos.Infrastructure.Projections.New
{
    public class MultiProjection<TKey,T> : PersistentProjection<TKey,T> where T : class, IReadModel, new() where TKey : IEquatable<TKey>
    {
        private TKey _key;
        public new TKey Key {
            set
            {
                _key = value;
                base.Key = new KeySelector(s => _key);
            } 
        }

        protected override void Reset(ref IObservable<GroupedObservable<StreamDetails, IEvent>> events)
        {
            events = events.StartWith(new GroupedObservable<StreamDetails, IEvent>
            {
                Key = new StreamDetails("Dummy"),
                Observable = Observable.Return(ResetState())
            });
        }

        public MultiProjection(IEventStore eventStore, IStateWriter writer, IReadRepository readRepository) 
            : base(eventStore, writer, readRepository) {}
    }
}