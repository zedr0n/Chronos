using System;
using System.Linq;
using Chronos.Infrastructure;
using Chronos.Infrastructure.Interfaces;
using Chronos.Infrastructure.Projections;
using Chronos.Infrastructure.Queries;
using NodaTime;

namespace Chronos.Core.Accounts.Projections
{
    public static class ProjectorExtensions
    {
        public static HistoricalProjector<T, TKey, TProjection> AsOf<T, TKey, TProjection>(
            this T projector, Instant date)
            where T : class, IProjector<TKey, TProjection>
            where TProjection : class, IProjection, IProjection<TKey>, new()
        {
            return new HistoricalProjector<T, TKey, TProjection>(projector.Repository,projector.Connection,projector.Writer)
                            .AsOf(projector,date);
        }
    }

    public class HistoricalProjector<T,TKey,TProjection> : IProjector where T : class, IProjector<TKey,TProjection>
                                                                      where TProjection : class,IProjection, IProjection<TKey>, new()
    {
        public IProjectionRepository Repository { get; }
        public IEventStoreConnection Connection { get; }
        public IProjectionWriter Writer { get; }
        private Subscription _subscription;
        private HistoricalKey<TKey> _key;
        private Instant _asOf;

        private T _projector;

        public void Start()
        {
            Connection.SubscribeToStream(_subscription.StreamName, _subscription.EventNumber, When);
        }

        public HistoricalProjector(IProjectionRepository repository, IEventStoreConnection connection, IProjectionWriter writer)
        {
            Repository = repository;
            Connection = connection;
            Writer = writer;
        }

        public HistoricalProjector<T,TKey,TProjection> AsOf(T projector, Instant date)
        {
            var historicalProjector = new HistoricalProjector<T, TKey, TProjection>(Repository, Connection,Writer)
            {
                _key = new HistoricalKey<TKey>
                {
                    Key = projector.Key,
                    AsOf = date
                },
                _projector = projector,
                _asOf = date
            };

            var projection = Repository.Find<HistoricalKey<TKey>, HistoricalProjection<TKey,TProjection>>(historicalProjector._key);
            if (projection == null)
                Repository.Add( new HistoricalProjection<TKey,TProjection>(projector.Key,date) );

            var afterEvent = projection?.Projection.LastEvent ?? -1;
            historicalProjector._subscription = new Subscription(projector.Subscription.StreamName, afterEvent, historicalProjector.When);

            return historicalProjector;
        }

        public void When(IEvent e)
        {
            if(e.Timestamp.CompareTo(_asOf) <= 0)
                Writer.UpdateOrThrow<HistoricalKey<TKey>,HistoricalProjection<TKey,TProjection>>(_key, v => _projector.When(e, v.Projection));
        }
    }
}