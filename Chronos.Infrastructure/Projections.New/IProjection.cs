using System;
using System.Runtime.InteropServices;
using Chronos.Infrastructure.Events;
using NodaTime;

namespace Chronos.Infrastructure.Projections.New
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