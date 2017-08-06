using Chronos.Infrastructure;
using Chronos.Infrastructure.Projections.New;
using SimpleInjector;

namespace Chronos.CrossCuttingConcerns.DependencyInjection
{
    public class ProjectionManager : IProjectionManager
    {
        private readonly Container _container;

        public ProjectionManager(Container container)
        {
            _container = container;
        }

        public IBaseProjectionExpression<T> Create<T>()
            where T : class, IReadModel, new()
        {
            return _container.GetInstance<IBaseProjectionExpression<T>>();
        }
    }
}