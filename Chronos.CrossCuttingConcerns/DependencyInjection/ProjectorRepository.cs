using System;
using Chronos.Infrastructure.Projections;
using SimpleInjector;

namespace Chronos.CrossCuttingConcerns.DependencyInjection
{
    public class ProjectorRepository : IProjectorRepository
    {
        private readonly Container _container;

        public ProjectorRepository(Container container)
        {
            _container = container;
        }

        public IProjector Get(Type t)
        {
            return  (IProjector) _container.GetInstance(typeof(IProjector<>).MakeGenericType(t));
        }
    }
}