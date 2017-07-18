using NodaTime;

namespace Chronos.Infrastructure.Queries
{

    public class HistoricalQuery<TQuery,TResult> : IQuery<TResult>
    {
        public IQuery<TResult> Query { get; private set; }
        public Instant AsOf { get; private set; }
    }
}