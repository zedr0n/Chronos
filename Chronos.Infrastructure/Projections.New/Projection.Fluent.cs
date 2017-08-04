using System;
using System.Reactive.Linq;
using Chronos.Infrastructure.Events;
using NodaTime;

namespace Chronos.Infrastructure.Projections.New
{
    public class ProjectionBuilder<T>
        where T : class, IReadModel,new()
    {
        private readonly IEventStoreSubscriptions _eventStore;
        private readonly IStateWriter _writer;
        private readonly IEventBus _eventBus;

        private Projection<T> _projection;
        private IObservable<StreamDetails> _streams;
        private bool _forEachStream;

        public ProjectionBuilder(IEventStoreSubscriptions eventStore, IStateWriter writer, IEventBus eventBus)
        {
            _eventStore = eventStore;
            _writer = writer;
            _eventBus = eventBus;

            _streams = _eventStore.Streams;
            //_projection = new Projection<T>(_eventStore,_writer,_eventBus);
        }

        public ProjectionBuilder<T> From<TAggregate>()
            where TAggregate : IAggregate
        {
            _streams = _streams.Where(s => s.SourceType == typeof(TAggregate).Name);
            return this;
        }

        public ProjectionBuilder<T> From<TAggregate>(Guid id) where TAggregate : IAggregate
        {
            From<TAggregate>();
            _streams = _streams.Where(s => s.Key == id);
            return this;
        }

        public ITransientProjection<T> Transient() => new TransientProjection<T>(_eventStore)
        {
            Streams = _streams
        };

        public ITransientProjection<T> AsOf(Instant date) => new HistoricalProjection<T>(_eventStore, date)
        {
            Streams = _streams
        };

        public ProjectionBuilder<T> ForEachStream()
        {
            _forEachStream = true;
            return this;
        } 
    }

    public partial class Projection<T>
    {
        public IProjection<T> From<TAggregate>() where TAggregate : IAggregate
        {
            Streams = Streams.Where(s => s.SourceType == typeof(TAggregate).Name);
            return this;
        }

        public IProjection<T> From<TAggregate>(Guid id) where TAggregate : IAggregate
        {
            From<TAggregate>();
            return new AggregateProjection<T>(this,id);
        }

        public ITransientProjection<T> Transient() => new TransientProjection<T>(this);
        public ITransientProjection<T> AsOf(Instant date) => new HistoricalProjection<T>(this, date);

        public IPartitionedProjection<T> ForEachStream() => new PersistentPartitionedProjection<T>(this);

        public IPersistentProjection<T> OutputState() => this as IPersistentProjection<T>;
        public IPersistentProjection<T> OutputState<TKey>(TKey key) where TKey : IEquatable<TKey>
        {
            return new PersistentProjection<TKey, T>(this, stream => key);
        }

    }
}