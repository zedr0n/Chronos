using System;
using Chronos.Infrastructure.Events;
using Chronos.Infrastructure.Interfaces;

namespace Chronos.Infrastructure.Projections
{
    public class Subscription
    {
        public string StreamName { get; }
        public int EventNumber { get; }
        public Action<IEvent> OnEvent { get; }

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
        IEventBus EventBus { get; }

        void When(IEvent e);
    }

    public interface IProjector<TKey, T> : IProjector  where T : class, IProjection
    {
        TKey Key { get; }
        Subscription Subscription { get; }
        void Start();
        void When(IEvent e, T p);
        IProjector<TKey, T> Assign<TAggregate>(TKey key) where TAggregate : class, IAggregate;

    }
}