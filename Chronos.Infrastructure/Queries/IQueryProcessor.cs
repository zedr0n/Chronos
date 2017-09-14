namespace Chronos.Infrastructure.Queries
{
    public interface IQueryProcessor
    {
        /// <summary>
        /// Return result of historical query
        /// </summary>
        /// <param name="query"></param>
        /// <typeparam name="TQuery">Query type</typeparam>
        /// <typeparam name="TResult">Type of query result</typeparam>
        /// <returns>Query result</returns>
        TResult Process<TQuery, TResult>(HistoricalQuery<TQuery> query)
            where TQuery : IQuery
            where TResult : class, IReadModel, new();
        /// <summary>
        /// Return result of query
        /// </summary>
        /// <param name="query"></param>
        /// <typeparam name="TQuery">Query type</typeparam>
        /// <typeparam name="TResult">Type of query result</typeparam>
        /// <returns>Query result</returns> 
        TResult Process<TQuery, TResult>(TQuery query)
            where TQuery : IQuery
            where TResult : class, IReadModel, new();
    }
}