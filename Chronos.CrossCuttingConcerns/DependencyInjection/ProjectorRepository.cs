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

        public T Get<T>() where T : class,IProjector
        {
            return   _container.GetInstance<T>();
        }
    }
}