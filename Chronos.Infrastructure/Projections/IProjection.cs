using NodaTime;

namespace Chronos.Infrastructure.Projections
{
    public interface IProjection
    {
        //Instant AsOf { get; set; }
        int LastEvent { get; set; }

        //T Copy<T>() where T : IProjection;
    }

    public interface IProjection<TKey> : IProjection
    {
        TKey Key { get; set; }
    }
}