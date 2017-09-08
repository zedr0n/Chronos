using Chronos.Infrastructure.Queries;

namespace Chronos.Core.Projections
{
    public class StatsQuery : IQuery<Stats>
    {
        public string Id { get; } = "Global";
    }
}