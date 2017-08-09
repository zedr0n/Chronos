namespace Chronos.Infrastructure.Queries
{
    public interface IQueryProcessor
    {
        TResult Process<TQuery, TResult>(HistoricalQuery<TQuery> query)
            where TQuery : IQuery
            where TResult : class, IReadModel, new();
        
        TResult Process<TQuery, TResult>(TQuery query)
            where TQuery : IQuery
            where TResult : class, IReadModel, new();
    }
}