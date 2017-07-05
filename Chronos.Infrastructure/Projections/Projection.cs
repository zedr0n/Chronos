using NodaTime;

namespace Chronos.Infrastructure.Projections
{
    public class Projection : IProjection
    {
        public Instant AsOf { get; set; }
    }
}