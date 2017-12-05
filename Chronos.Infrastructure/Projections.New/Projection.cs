using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using Chronos.Infrastructure.Events;
using Chronos.Infrastructure.Interfaces;
using NodaTime;

namespace Chronos.Infrastructure.Projections.New
{
    public class Projection : IProjection
    {
        private readonly IEventStore _eventStore;
        protected Guid Timeline => _eventStore.Timeline.TimelineId;

        private IObservable<StreamDetails> _streams;
        
        public Selector<StreamDetails> Selector { get; set; } = new Selector<StreamDetails>();

        private IDisposable _subscription;

        protected Projection(IEventStore eventStore)
        {
            _eventStore = eventStore;
            _eventStore.Alerts.OfType<ReplayCompleted>().Subscribe(e => Start(true));
        }

        protected virtual void When(StreamDetails stream, IList<IEvent> e)
        {
        }
        
        protected StateReset ResetState() => 
            new StateReset
            {
                Timestamp = Instant.MinValue
            };

        private void Unsubscribe()
        {
            _subscription?.Dispose();
        }

        protected virtual int GetVersion(StreamDetails stream)
        {
            return -1;
        }

        protected virtual void Reset(ref IObservable<GroupedObservable<StreamDetails, IList<IEvent>>> events)
        {
            events = events.StartWith(new GroupedObservable<StreamDetails, IList<IEvent>>
            {
                Key = new StreamDetails("Dummy"),
                Observable = Observable.Return(new List<IEvent> { ResetState() })
            });
        }

        protected virtual void Register(IObservable<StreamDetails> streams) {}
        
        public virtual void Start(bool reset = false)
        {
            Unsubscribe();

            _streams = _eventStore.GetLiveStreams().Where(Selector);
            Register(_streams);
            var requests = _streams.Select(s => new StreamRequest(s, reset ? 0 : GetVersion(s)+1));
            
            var events = _eventStore.GetEventsBuffered(requests)
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