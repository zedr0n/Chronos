using System;
using Chronos.Infrastructure.Events;

namespace Chronos.Infrastructure.Projections
{
    public interface IProjector : IConsumer
    {
    }

    public interface IProjector<T> : IProjector
        where T : IProjection
    {
        void UpdateProjection(IEvent e, Action<T> action, Func<T, bool> where);
    }
}