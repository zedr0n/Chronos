using Chronos.Infrastructure.Projections.New;

namespace Chronos.Infrastructure.Queries
{
    public sealed class HistoricalQueryHandler<TQuery, TResult> : IQueryHandler<HistoricalQuery<TQuery,TResult>, TResult>
        where TResult : class, IReadModel, new()
        where TQuery : IQuery<TResult>

    {
        public HistoricalQueryHandler(IQueryHandler<TQuery, TResult> queryHandler)
        {
            Projection = queryHandler.Projection;
        }

        public IProjection<TResult> Projection { get; }

        public TResult Handle(HistoricalQuery<TQuery,TResult> query)
        {
            var projection = Projection.AsOf(query.AsOf);
            projection.Start();

            return projection.State;
        }
    }
}