namespace Chronos.Infrastructure.Projections
{
    public interface IProjection
    {
    }

    public interface ITransientProjection<T> : IProjection where T : class,IReadModel, new()
    {
        T State { get; }
    }

    public interface IPersistentProjection<T> : IProjection where T : class, IReadModel, new()
    {
    }
}