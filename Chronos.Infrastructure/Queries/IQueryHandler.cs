using Chronos.Infrastructure.Projections.New;

namespace Chronos.Infrastructure.Queries
{
    /// <typeparam name="TQuery">Query type</typeparam>
    /// <typeparam name="TResult">Query result type</typeparam>
    public interface IQueryHandler<in TQuery, TResult> //where TQuery : IQuery<TResult>
        where TResult : class, IReadModel, new()
    {
        /// <summary>
        /// Fluent expression associated with the handlers
        /// </summary>
        IProjectionExpression<TResult> Expression { get; }

        /// <summary>
        /// Query handler processor ( can be overriden via decorators )
        /// </summary>
        /// <param name="query"> Query object </param>
        /// <returns>Query result</returns>
        TResult Handle(TQuery query);
    }
}