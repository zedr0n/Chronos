using System;
using NodaTime;

namespace Chronos.Infrastructure.Projections.New
{
    public interface IProjectionFrom<T> where T : class, IReadModel, new()
    {
        IProjection<T> From<TAggregate>() where TAggregate : IAggregate;
        IProjection<T> From<TAggregate>(Guid id) where TAggregate : IAggregate;
    }

    public interface IProjection<T> where T : class, IReadModel, new()
    {
        ITransientProjection<T> Transient();
        ITransientProjection<T> AsOf(Instant date);
        IPersistentProjection<T> OutputState<TKey>(TKey key) where TKey : IEquatable<TKey>;
        IPartitionedProjection<T> ForEachStream();
    }

    public interface IPartitionedProjection<T> where T : class, IReadModel, new()
    {
        IPersistentProjection<T> OutputState();
        void Start();
    }

    public interface ITransientProjection<T>  where T : class,IReadModel, new()
    {
        T State { get; }
        void Start();
    }

    public interface IPersistentProjection<T> where T : class, IReadModel, new()
    {
        void Start();
    }
}