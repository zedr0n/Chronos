using Chronos.Infrastructure.Projections.New;

namespace Chronos.Infrastructure.Queries
{
    public sealed class HistoricalQueryHandler<TQuery, TResult> : IQueryHandler<HistoricalQuery<TQuery,TResult>, TResult>
        where TResult : class, IReadModel, new()
        where TQuery : IQuery<TResult>
    {
        public HistoricalQueryHandler(IQueryHandler<TQuery, TResult> queryHandler)
        {
            Expression = queryHandler.Expression.Clone();
        }
        public IProjectionExpression<TResult> Expression { get; }

        public TResult Handle(HistoricalQuery<TQuery,TResult> query)
        {
            var projection = Expression.AsOf(query.AsOf).Compile();
            return projection.State;
        }
    }
}