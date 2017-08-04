using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reactive.Linq;
using Chronos.Infrastructure.Events;
using Chronos.Infrastructure.Interfaces;

namespace Chronos.Infrastructure.Projections.New
{
    public partial class Projection<T> : IProjectionFrom<T>, IProjection<T> where T : class, IReadModel, new()
    {
        private readonly IEventStoreSubscriptions _eventStore;
        private readonly IStateWriter _writer;
        private readonly IEventBus _eventBus;

        public IObservable<StreamDetails> Streams { get; set; }

        private int _lastEvent = -1;

        private IDisposable _streamsSubscription;
        private readonly Dictionary<string, IDisposable> _eventSubscriptions = new Dictionary<string, IDisposable>();

        protected Projection(Projection<T> projection)
            : this(projection._eventStore, projection._writer, projection._eventBus)
        {
            Streams = projection.Streams;
        }

        public Projection(IEventStoreSubscriptions eventStore, IStateWriter writer, IEventBus eventBus)
        {
            _eventStore = eventStore;
            _writer = writer;
            _eventBus = eventBus;

            Streams = _eventStore.Streams;
            _eventBus.Subscribe<ReplayCompleted>(When);
        }

        public Projection(IEventStoreSubscriptions eventStore)
        {
            _eventStore = eventStore;

            Streams = _eventStore.Streams;
        }

        private void When(ReplayCompleted e)
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
            _eventSubscriptions[stream.Name] = GetEvents(stream).Subscribe(e => When(stream, e));
        }

        protected virtual IObservable<IEvent> GetEvents(StreamDetails stream)
        {
            return _eventStore.GetEvents(stream, _lastEvent);
        }

        protected void Write<TKey>(TKey key, IEvent e)
            where TKey : IEquatable<TKey>
        {
            _writer.Write<TKey,T>(key,x => x.When(e));
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