using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reactive.Linq;
using Chronos.Infrastructure.Interfaces;

namespace Chronos.Infrastructure.Projections.New
{
    public class Projection : IProjection
    {
        private readonly IEventStoreSubscriptions _eventStore;

        public IObservable<StreamDetails> Streams { get; set; }
        
        private int _lastEvent = -1;

        private IDisposable _streamsSubscription;
        private readonly Dictionary<string, IDisposable> _eventSubscriptions = new Dictionary<string, IDisposable>();

        protected Projection(IEventStoreSubscriptions eventStore)
        {
            _eventStore = eventStore;

            Streams = _eventStore.GetStreams().Where(s => !s.Name.Contains("Saga"));
        }

        public void OnReplay()
        {
            _lastEvent = -1;
            Start();
        }
        
        protected virtual void When(StreamDetails stream, IEvent e)
        {
            if (e.EventNumber > _lastEvent)
                _lastEvent = e.EventNumber;
        }

        private void OnStreamAdded(StreamDetails stream)
        {
            Debug.Assert(!_eventSubscriptions.ContainsKey(stream.Name));
            _eventSubscriptions[stream.Name] = GetEvents(stream)//.SubscribeOn(Scheduler.Default)
                .Subscribe(e => When(stream, e));
        }
        
        protected virtual IObservable<IEvent> GetEvents(StreamDetails stream)
        {
            return _eventStore.GetEvents(stream, _lastEvent);
        }

        private void Unsubscribe()
        {
            _streamsSubscription?.Dispose();
            foreach (var s in _eventSubscriptions.Values)
                s.Dispose();
            _eventSubscriptions.Clear();
        }

        public void Start()
        {
            Unsubscribe();

            _streamsSubscription = Streams.Subscribe(OnStreamAdded);
        }
    }
}