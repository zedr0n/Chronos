using System;

namespace Chronos.Infrastructure.Projections
{
    public interface IProjectorRepository
    {
        T Get<T>() where T : class,IProjector;
    }
}