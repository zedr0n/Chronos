using System;
using Chronos.Infrastructure.Events;
using Chronos.Infrastructure.Interfaces;
using NodaTime;

namespace Chronos.Infrastructure.Projections
{
    public static class ProjectorExtensions
    {
        public static IProjector AsOf<TKey, TProjection>(
            this IProjector<TKey,TProjection> projector, Instant date)
            where TProjection : class, IProjection<TKey>, new()
            where TKey : IEquatable<TKey>
        {
            if (date == Instant.MaxValue)
                return projector;
            else
                return new HistoricalProjector<TKey, TProjection>(projector.Writer,projector.Repository,projector.Connection, projector.EventBus)
                            .AsOf(projector,date);
        }
    }

    public class HistoricalProjector<TKey,TProjection> : ProjectorBase<HistoricalKey<TKey>,HistoricalProjection<TKey,TProjection>>
                                                                      where TProjection : class,IProjection<TKey>, new()
                                                                      where TKey : IEquatable<TKey>
    {
        private Instant _asOf;
        private IProjector<TKey,TProjection> _projector;

        public HistoricalProjector(IProjectionWriter writer, IProjectionRepository repository, IEventStoreConnection connection, IEventBus eventBus)
            : base(writer, repository, connection, eventBus) { }

        public HistoricalProjector<TKey,TProjection> AsOf(IProjector<TKey,TProjection> projector, Instant date)
        {
            var historicalProjector = new HistoricalProjector<TKey, TProjection>(Writer, Repository, Connection,EventBus)
            {
                Key = new HistoricalKey<TKey>(projector.Key,date),
                _projector = projector,
                _asOf = date
            };

            var projection = Repository.Find<TKey,TProjection>(historicalProjector.Key);

            var afterEvent = projection?.LastEvent ?? -1;
            historicalProjector.Subscription = new Subscription(projector.Subscription.StreamName, afterEvent,
                e =>
                {
                    if (e.Timestamp <= _asOf)
                        historicalProjector.When(e);
                });

            return historicalProjector;
        }

        public override void When(IEvent e, HistoricalProjection<TKey, TProjection> p) => _projector.When(e, p.Projection);
    }
}