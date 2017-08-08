using NodaTime;

namespace Chronos.Infrastructure.Queries
{
    public class HistoricalQuery<TQuery> where TQuery : IQuery
    {
        public TQuery Query { get; set; }
        public Instant AsOf { get; set; }
    }
}