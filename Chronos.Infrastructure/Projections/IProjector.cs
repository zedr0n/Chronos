using System;
using Chronos.Infrastructure.Events;
using Chronos.Infrastructure.Interfaces;

namespace Chronos.Infrastructure.Projections
{
    public class Subscription
    {
        public string StreamName { get; private set; }
        public int EventNumber { get; private set; }
        public Action<IEvent> OnEvent { get; private set; }

        public Subscription(string streamName, int eventNumber, Action<IEvent> onEvent)
        {
            StreamName = streamName;
            EventNumber = eventNumber;
            OnEvent = onEvent;
        }
    }

    public interface IProjector : IConsumer
    {
        IProjectionRepository Repository { get; }
        IEventStoreConnection Connection { get; }
        IProjectionWriter Writer { get; }

        void When(IEvent e);
    }

    public interface IKeyedProjector<TKey> : IProjector
    {
        TKey Key { get; }
    }

    public interface IProjector<TKey, T> : IKeyedProjector<TKey> where T : class, IProjection
    {
        Subscription Subscription { get; }
        void When(IEvent e, T p);
    }

    public interface IProjector<T> : IProjector
        where T : IProjection
    {
        void UpdateProjection(IEvent e, Action<T> action, Func<T, bool> where);
    }
}