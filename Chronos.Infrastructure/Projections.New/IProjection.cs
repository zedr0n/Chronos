using System;
using System.Collections.Generic;
using Chronos.Infrastructure.Interfaces;
using NodaTime;

namespace Chronos.Infrastructure.Projections.New
{
    public interface IProjectionHandler<T> where T : class, IReadModel, new()
    {
        void When(T s, IEvent e);
    }

    public interface IProjection<T> where T : class, IReadModel, new()
    {
        IProjection<T> From(IEnumerable<string> streams);

        void Start();
        ITransientProjection<T> AsOf(Instant date);
        IProjection<T> When(IProjectionHandler<T> handler);

        IProjection<TKey, T> OutputState<TKey>(TKey key) where TKey : IEquatable<TKey>;

    }

    public interface ITransientProjection<T> : IProjection<T> where T : class,IReadModel, new()
    {
        new ITransientProjection<T> From(IEnumerable<string> streams);
        T State { get; }
    }

    public interface IProjection<TKey,T> : IProjection<T> where T : class, IReadModel, new()
                                                          where TKey : IEquatable<TKey>
    {
        new IProjection<TKey,T> When(IProjectionHandler<T> handler);
        new IProjection<TKey, T> From(IEnumerable<string> streams);
        IProjection<TKey,T> ForEachStream(Func<string, TKey> map);
    }
}