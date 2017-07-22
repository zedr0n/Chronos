using System;
using System.Collections.Generic;
using NodaTime;

namespace Chronos.Infrastructure.Projections.New
{
    public interface IProjectionFrom<T> where T : class, IReadModel, new()
    {
        IProjection<T> From<TAggregate>() where TAggregate : IAggregate;
        IProjection<T> From<TKey, TAggregate>(TKey key) where TAggregate : IAggregate;
        IProjection<T> From(string streamName);
        IProjection<T> From(IEnumerable<string> streams);
    }

    public interface IProjection<T> where T : class, IReadModel, new()
    {
        ITransientProjection<T> Transient();
        ITransientProjection<T> AsOf(Instant date);
        IPersistentProjection<T> OutputState<TKey>(TKey key) where TKey : IEquatable<TKey>;
        IPersistentProjection<T> OutputState<TKey>(Func<string,TKey> map) where TKey : IEquatable<TKey>;

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

    public interface IProjection<TKey,T> where T : class, IReadModel, new()
                                                          where TKey : IEquatable<TKey>
    {
        IProjection<TKey,T> ForEachStream(Func<string, TKey> map);
    }
}