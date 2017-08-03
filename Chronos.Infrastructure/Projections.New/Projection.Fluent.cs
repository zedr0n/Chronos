using System;
using System.Reactive.Linq;
using NodaTime;

namespace Chronos.Infrastructure.Projections.New
{
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