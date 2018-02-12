namespace Chronos.Infrastructure.Projections
{
    public interface IProjection
    {
    }

    public interface IProjection<T> : IProjection where T : IReadModel
    {
        void Start(bool reset);
    }

    public interface ITransientProjection<T> : IProjection where T : class,IReadModel, new()
    {
        T State { get; }
    }

    public interface IPersistentProjection<T> : IProjection where T : class, IReadModel, new()
    {
    }
}