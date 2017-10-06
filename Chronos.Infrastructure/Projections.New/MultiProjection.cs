using System;
using System.Reactive.Linq;
using Chronos.Infrastructure.Events;
using Chronos.Infrastructure.Interfaces;

namespace Chronos.Infrastructure.Projections.New
{
    public class MultiProjection<TKey,T> : PersistentProjection<TKey,T> where T : class, IReadModel, new() where TKey : IEquatable<TKey>
    {
        private TKey _key;
        public TKey Key {
            get => _key;
            set
            {
                _key = value;
                KeyFunc = s => Key;
            } }

        protected override void Reset(ref IObservable<GroupedObservable<StreamDetails, IEvent>> events)
        {
            events = events.StartWith(new GroupedObservable<StreamDetails, IEvent>
            {
                Key = new StreamDetails("Dummy"),
                Observable = Observable.Return(new StateReset())
            });
        }

        public MultiProjection(IEventStore eventStore, IStateWriter writer, IReadRepository readRepository) 
            : base(eventStore, writer, readRepository) {}
    }
}