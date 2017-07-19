using NodaTime;

namespace Chronos.Infrastructure.Projections
{
    public interface IProjection
    {
        int LastEvent { get; set; }
        bool Live { get; set; }
    }

    public interface IProjection<TKey> : IProjection
    {
        TKey Key { get; set; }
    }
}