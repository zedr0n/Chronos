using NodaTime;

namespace Chronos.Infrastructure.Projections
{
    public class ProjectionBase : IProjection
    {
        public int LastEvent { get; set; } = -1;
        public bool Live { get; set; }
    }

    public class ProjectionBase<TKey> : ProjectionBase,IProjection<TKey>
    {
        public TKey Key { get; set; }
    }
}