using NodaTime;

namespace Chronos.Infrastructure.Projections
{
    public interface IProjection
    {
        Instant AsOf { get; set; }
        int LastEvent { get; set; }
    }
}