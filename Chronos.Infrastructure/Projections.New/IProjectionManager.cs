using System;

namespace Chronos.Infrastructure.Projections.New
{
    public interface IProjectionManager
    {
        IProjection<T> Create<T>() where T : class, IReadModel, new();
    }
}