using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Chronos.Infrastructure.Events;
using Chronos.Infrastructure.Interfaces;
using System.Reactive.Linq;

namespace Chronos.Infrastructure.Projections.New
{

    public partial class Projection<T> : IProjectionFrom<T>, IProjection<T> where T : class, IReadModel, new()
    {
        private readonly IEventStoreSubscriptions _eventStore;
        private readonly IStateWriter _writer;
        private readonly IEventBus _eventBus;

        private Func<StreamDetails,bool> _from = s => true;
        private int _lastEvent = -1;

        private IDisposable _streamsSubscription;
        private readonly Dictionary<string, IDisposable> _eventSubscriptions = new Dictionary<string, IDisposable>();
        private Projection(Projection<T> projection)
            : this(projection._eventStore, projection._writer, projection._eventBus)
        {
            _from = projection._from;
        }

        public Projection(IEventStoreSubscriptions eventStore, IStateWriter writer, IEventBus eventBus)
        {
            _eventStore = eventStore;
            _writer = writer;
            _eventBus = eventBus;
        }

        public IProjection<T> From<TAggregate>() where TAggregate : IAggregate
        {
            _from = s => s.SourceType == typeof(TAggregate).Name;
            return this;
        }

        public IProjection<T> From<TAggregate>(Guid id) where TAggregate : IAggregate
        {
            _from = s => s.SourceType == typeof(TAggregate).Name && s.Id == id;
            return this;
        }

        private void When(ReplayCompleted e)
        {
            _lastEvent = -1;
            Start();
        }
        protected virtual void When(IEvent e) { }
        protected virtual void When(StreamDetails stream, IEvent e) => When(e);

        private void ReadEventsFromStream(StreamDetails stream)
        {
            Debug.Assert(!_eventSubscriptions.ContainsKey(stream.Name));

            _eventSubscriptions[stream.Name] = _eventStore.GetEvents(stream, _lastEvent)
                                                          .Subscribe(e => When(stream,e));
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

            _streamsSubscription = _eventStore.Streams
                .Where(_from)
                .Subscribe(ReadEventsFromStream);

            _eventBus.Subscribe<ReplayCompleted>(When);
        }
    }
}