using NodaTime;

namespace Chronos.Infrastructure.Queries
{

    public class HistoricalQuery<TQuery,TResult> : IQuery<TResult>
        where TQuery : IQuery<TResult>
    {
        public TQuery Query { get; set; }
        public Instant AsOf { get; set; }
    }
}