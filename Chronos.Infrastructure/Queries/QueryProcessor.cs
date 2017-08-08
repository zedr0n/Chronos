namespace Chronos.Infrastructure.Queries
{
    public class QueryProcessor : IQueryProcessor
    {
        public TResult Process<TResult>(IQuery<TResult> query)
        {
            return default(TResult);
        }
    }
}