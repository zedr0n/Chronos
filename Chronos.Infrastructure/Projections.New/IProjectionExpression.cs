using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using NodaTime;

namespace Chronos.Infrastructure.Projections.New
{
    public interface IBaseProjectionExpression<T> where T : class, IReadModel, new()
    {
        IProjectionExpression<T> From<TAggregate>()
            where TAggregate : IAggregate;
        
        IProjectionExpression<T> From<TAggregate>(Guid id) where TAggregate : IAggregate;
    }

    public interface IProjectionExpression<T> where T : class, IReadModel, new()
    {
        IProjectionExpression<T> Clone();

        IProjectionExpression<T> ForEachStream();

        ITransientProjectionExpression<T> Transient();
        ITransientProjectionExpression<T> AsOf(Instant date);

        IPersistentProjectionExpression<T> OutputState();

        IPersistentProjectionExpression<T> OutputState<TKey>(TKey key)
            where TKey : IEquatable<TKey>;

        void Compile();
    }

    public interface ITransientProjectionExpression<T> : IProjectionExpression<T>
        where T : class, IReadModel, new()
    {
        new ITransientProjection<T> Compile();
    }

    public interface IPersistentProjectionExpression<T> : IProjectionExpression<T>
        where T : class, IReadModel, new()
    {
        new IPersistentProjection<T> Compile();
    }
}