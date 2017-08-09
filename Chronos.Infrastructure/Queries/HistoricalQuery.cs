using NodaTime;

namespace Chronos.Infrastructure.Queries
{
    public interface IHistoricalQuery {} 
    public class HistoricalQuery<TQuery> : IHistoricalQuery where TQuery : IQuery
    {
        public TQuery Query { get; set; }
        public Instant AsOf { get; set; }
    }
}