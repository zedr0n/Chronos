using System;

namespace Chronos.Infrastructure.Projections.New
{
    public interface IProjectionManager
    {
        IProjectionFrom<T> Create<T>() where T : class, IReadModel, new();
    }
}