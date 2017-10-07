using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text.RegularExpressions;
using Chronos.Infrastructure.Events;
using Chronos.Infrastructure.Interfaces;

namespace Chronos.Infrastructure.Projections.New
{
    public class Projection : IProjection
    {
        private readonly IEventStore _eventStore;

        private IObservable<StreamDetails> _streams;
        
        public Selector<StreamDetails> Selector { get; set; } = new Selector<StreamDetails>();

        private IDisposable _subscription;

        protected Projection(IEventStore eventStore)
        {
            _eventStore = eventStore;
            _eventStore.Alerts.OfType<ReplayCompleted>().Subscribe(e => Start(true));
        }

        protected virtual void When(StreamDetails stream, IEvent e)
        {
        }

        private void Unsubscribe()
        {
            _subscription?.Dispose();
        }

        protected virtual int GetVersion(StreamDetails stream)
        {
            return -1;
        }

        protected virtual void Reset(ref IObservable<GroupedObservable<StreamDetails,IEvent>> events) {}
        
        public void Start(bool reset = false)
        {
            Unsubscribe();

            _streams = _eventStore.GetLiveStreams().Where(Selector);
            var requests = _streams.Select(s => new StreamRequest(s, reset ? 0 : GetVersion(s)+1));
            
            var events = _eventStore.GetEvents(requests)
                .Select(x => new GroupedObservable<StreamDetails,IEvent>
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