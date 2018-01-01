namespace Chronos.Infrastructure.Projections
{
    public interface IProjectionManager
    {
        IBaseProjectionExpression<T> Create<T>()
            where T : class, IReadModel, new();
    }
}