using System;
using System.Diagnostics;
using System.Reflection;
using Chronos.Infrastructure.Events;
using Chronos.Infrastructure.Interfaces;

namespace Chronos.Infrastructure.Projections
{
    public abstract class ProjectorBase<TKey, T> : IProjector<TKey, T> where T : class, IProjection<TKey>, new()
                                                                       where TKey : IEquatable<TKey>
    {

        public IProjectionWriter Writer { get; }
        public IProjectionRepository Repository { get;}
        public IEventStoreConnection Connection { get; }
        public IEventBus EventBus { get; }

        public TKey Key { get; protected set; }

        public Subscription Subscription { get; protected set; }

        protected ProjectorBase(IProjectionWriter writer, IProjectionRepository repository, IEventStoreConnection connection, IEventBus eventBus)
        {
            Writer = writer;
            Repository = repository;
            Connection = connection;
            EventBus = eventBus;
        }

        public IProjector<TKey,T> Assign<TAggregate>(TKey key) where TAggregate : class, IAggregate
        {
            var projector = MemberwiseClone() as ProjectorBase<TKey, T>;
            Debug.Assert(projector != null, "projector != null");
            projector.Key = key;

            var projection = Repository.Find<TKey,T>(key);
            var afterEvent = projection?.LastEvent ?? -1;
            projector.Subscription = new Subscription(StreamExtensions.StreamName<TKey,TAggregate>(key), afterEvent, projector.When);
            return projector;
        }

        public void Start()
        {
            var projection = Repository.Find<TKey, T>(Key);
            if(projection == null)
                Writer.Add(new T { Key = Key, Live = true });
            else if (projection.Live) return;
            else projection.Live = true;

            EventBus.Subscribe<ReplayCompleted>(e => Reset());
            Connection.SubscribeToStream(Subscription.StreamName, Subscription.EventNumber, Subscription.OnEvent);
        }

        private void Reset()
        {
            Connection.DropSubscription(Subscription.StreamName,Subscription.OnEvent);
            Connection.SubscribeToStream(Subscription.StreamName, -1, Subscription.OnEvent);
        }

        public abstract void When(IEvent e, T p);

        public void When(IEvent e)
        {
            Writer.UpdateOrThrow<TKey, T>(Key, v => When(e, v));
        }
    }
}