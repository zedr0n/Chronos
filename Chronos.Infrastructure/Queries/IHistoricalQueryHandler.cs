namespace Chronos.Infrastructure.Queries
{
    public interface IHistoricalQueryHandler<TQuery, TResult> : IQueryHandler<HistoricalQuery<TQuery>,TResult>
        where TResult : class, IReadModel, new() 
        where TQuery : IQuery
    {
        
    }
}