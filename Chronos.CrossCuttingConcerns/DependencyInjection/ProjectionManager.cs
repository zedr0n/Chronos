using Chronos.Core.Accounts;
using Chronos.Core.Projections;
using Chronos.Infrastructure;
using Chronos.Infrastructure.Projections;
using SimpleInjector;

namespace Chronos.CrossCuttingConcerns.DependencyInjection
{
    public class ProjectionManager : IProjectionManager
    {
        private readonly Container _container;

        public ProjectionManager(Container container)
        {
            _container = container;

            //var stats =Create<Stats>().From<Account>().OutputState("Global");
            
            //stats.Invoke();
        }

        public IBaseProjectionExpression<T> Create<T>()
            where T : class, IReadModel, new()
        {
            return _container.GetInstance<IBaseProjectionExpression<T>>();
        }
    }
}