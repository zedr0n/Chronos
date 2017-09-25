using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using Chronos.Infrastructure.Events;
using Chronos.Infrastructure.Interfaces;

namespace Chronos.Infrastructure.Projections.New
{
    public class Projection : IProjection
    {
        private readonly IEventStoreSubscriptions _eventStore;

        private IObservable<StreamDetails> _streams;
        private readonly List<StreamDetails> _observedStreams = new List<StreamDetails>();
        public Selector<StreamDetails> Selector { get; set; } = new Selector<StreamDetails>();

        private readonly Dictionary<int, int> _lastEvents = new Dictionary<int, int>();

        private IDisposable _streamsSubscription;
        private readonly Dictionary<string, IDisposable> _eventSubscriptions = new Dictionary<string, IDisposable>();

        protected Projection(IEventStoreSubscriptions eventStore)
        {
            _eventStore = eventStore;
        }

        public void OnReplay()
        {
            Reset();
            Start();
        }

        private void Reset()
        {
            //foreach(var s in _observedStreams)
            //    When(s, new StateReset());
            _lastEvents.Clear();
        }
        
        protected virtual void When(StreamDetails stream, IEvent e)
        {
            var hash = stream.Name.HashString();
            Debug.Assert(_lastEvents.ContainsKey(hash));
            if (e.Version > _lastEvents[hash])
                _lastEvents[hash] = e.Version;
        }

        private void OnStreamAdded(StreamDetails stream)
        {
            Debug.Assert(!_eventSubscriptions.ContainsKey(stream.Name));
            if (!_lastEvents.ContainsKey(stream.Name.HashString()))
            {
                var resetState = !_lastEvents.Any();
                _lastEvents[stream.Name.HashString()] = -1;
                if(resetState)
                    When(stream, new StateReset());
            }

            _eventSubscriptions[stream.Name] = GetEvents(stream)//.SubscribeOn(Scheduler.Default)
                .Subscribe(e => When(stream, e));
            _observedStreams.Add(stream);
        }
        
        protected virtual IObservable<IEvent> GetEvents(StreamDetails stream)
        {
            return _eventStore.GetEvents(stream, _lastEvents[stream.Name.HashString()]);
        }

        private void Unsubscribe()
        {
            _observedStreams.Clear();
            _streamsSubscription?.Dispose();
            foreach (var s in _eventSubscriptions.Values)
                s.Dispose();
            _eventSubscriptions.Clear();
        }

        public void Start()
        {
            Unsubscribe();
            _streams = _eventStore.GetStreams().Where(Selector);
            
            _streamsSubscription = _streams.Subscribe(OnStreamAdded);
        }
    }
}