using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using Chronos.Infrastructure.Events;
using Chronos.Infrastructure.Interfaces;
using Chronos.Infrastructure.Projections;
using NodaTime;

namespace Chronos.Infrastructure.ProjectionServices
{
    public class BaseProjection<T> : IProjection<T>
        where T : IReadModel
    {
        private readonly IEventStore _eventStore;
        private readonly IStreamSelector<T> _selector;
        private readonly IVersionProvider<T> _versionProvider;

        private static StateReset ResetState() => 
            new StateReset
            {
                Timestamp = Instant.MinValue
            };
        
        private IDisposable _subscription;

        public BaseProjection(IEventStore eventStore, IStreamSelector<T> selector, IVersionProvider<T> versionProvider)
        {
            _eventStore = eventStore;
            _selector = selector;
            _versionProvider = versionProvider;
        }

        protected virtual void When(StreamDetails s, IList<IEvent> events)
        {
        }

        protected virtual void When(StreamDetails s, IEvent e)
        {
            
        }

        protected virtual void Reset(ref IObservable<GroupedObservable<StreamDetails, IList<IEvent>>> events)
        {
            events = events.StartWith(new GroupedObservable<StreamDetails, IList<IEvent>>
            {
                Key = new StreamDetails("Dummy"),
                Observable = Observable.Return(new List<IEvent> { ResetState() })
            });
        }

        public virtual void Start(bool reset)
        {
            _subscription.Dispose();

            var streams = _selector.Streams.Select(s => new StreamDetails(s)
            {
                Version = _versionProvider.Get(s)
            });

            var bufferedEvents = _eventStore.GetEvents(streams);
            
            var events = _eventStore.GetEventsBuffered(streams)
                .Select(x => new GroupedObservable<StreamDetails,IList<IEvent>>
                {
                    Key = x.Key,
                    Observable = x.AsObservable()
                });

            if (reset)
                Reset(ref events); 
            
            _subscription = events.Subscribe(x =>
                x.Observable.Subscribe(e => When(x.Key, e)));
        }
    }
}