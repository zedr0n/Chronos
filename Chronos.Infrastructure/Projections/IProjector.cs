using System;
using Chronos.Infrastructure.Events;
using NodaTime;

namespace Chronos.Infrastructure.Projections
{
    public interface IProjector : IConsumer
    {
    }

    public interface IProjector<T> : IProjector
        where T : IProjection
    {
        void UpdateProjection(IEvent e, Action<T> action, Func<T, bool> where);
        void AddProjection(T projection,Func<T,bool> where);
    }
}