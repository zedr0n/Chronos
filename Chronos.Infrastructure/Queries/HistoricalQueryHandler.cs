using Chronos.Infrastructure.Projections;

namespace Chronos.Infrastructure.Queries
{
    public sealed class HistoricalQueryHandler<TQuery, TResult> : IHistoricalQueryHandler<TQuery,TResult>
        where TResult : class, IReadModel, new()
        where TQuery : IQuery<TResult>
    {
        public HistoricalQueryHandler(IQueryHandler<TQuery, TResult> queryHandler)
        {
            Expression = queryHandler.Expression.Clone();
        }
        public IProjectionExpression<TResult> Expression { get; }

        public TResult Handle(HistoricalQuery<TQuery> query)
        {
            var projection = Expression.AsOf(query.AsOf).Invoke();
            return projection.State;
        }
    }
}