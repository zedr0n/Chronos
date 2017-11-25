using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using NodaTime;

namespace Chronos.Infrastructure.Projections.New
{
    public interface IBaseProjectionExpression<T> where T : class, IReadModel, new()
    {
        /// <summary>
        /// Configure the projection to use aggregate streams
        /// </summary>
        /// <typeparam name="TAggregate">Aggregate type</typeparam>
        /// <returns>Projection expression</returns>
        IProjectionExpression<T> From<TAggregate>()
            where TAggregate : IAggregate;
        
        /// <summary>
        /// Configure the projection to use a single aggregate stream
        /// </summary>
        /// <param name="id">Aggregate id</param>
        /// <typeparam name="TAggregate">Aggregate type</typeparam>
        /// <returns>Projection expression</returns>
        IProjectionExpression<T> From<TAggregate>(Guid id) where TAggregate : IAggregate;
    }

    public interface IProjectionExpression<T>  where T : class, IReadModel, new()
    {
        /// <summary>
        /// Create a copy of the projection expression
        /// </summary>
        /// <returns>Projection copy as base</returns>
        IProjectionExpression<T> Clone();

        /// <summary>
        /// Partition the projection by stream
        /// </summary>
        /// <returns>Partitioned projection expression</returns>
        IProjectionExpression<T> ForEachStream();

        /// <summary>
        /// Include events from specified aggregate
        /// </summary>
        /// <typeparam name="TAggregate"></typeparam>
        /// <returns></returns>
        IProjectionExpression<T> Include<TAggregate>() where TAggregate : IAggregate;
        
        ITransientProjectionExpression<T> Transient();
        /// <summary>
        /// Configure the historical projection
        /// </summary>
        /// <param name="date">Last date for the events to be processed by the projection</param>
        /// <returns>Transient projection expression</returns>
        ITransientProjectionExpression<T> AsOf(Instant date);

        //IPersistentProjectionExpression<T> OutputState<TAggregate>() where TAggregate : IAggregate;
        IPersistentProjectionExpression<T> OutputState();

        IPersistentProjectionExpression<T> OutputState<TKey>(TKey key)
            where TKey : IEquatable<TKey>;

        void Invoke();
    }

    public interface ITransientProjectionExpression<T> : IProjectionExpression<T>
        where T : class, IReadModel, new()
    {
        new ITransientProjection<T> Invoke();
    }

    public interface IPersistentProjectionExpression<T> : IProjectionExpression<T>
        where T : class, IReadModel, new()
    {
        new IPersistentProjection<T> Invoke();
    }
}