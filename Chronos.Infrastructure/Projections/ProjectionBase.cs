using NodaTime;

namespace Chronos.Infrastructure.Projections
{
    public class ProjectionBase
    {
        public int LastEvent { get; set; } = -1;
    }

    public class ProjectionBase<TKey> : ProjectionBase
    {
        public TKey Key { get; set; }
    }
}