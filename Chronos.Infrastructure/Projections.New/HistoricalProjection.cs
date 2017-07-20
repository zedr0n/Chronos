using System;
using Chronos.Infrastructure.Interfaces;
using NodaTime;

namespace Chronos.Infrastructure.Projections.New
{
    public class HistoricalProjection<TKey,T> : ProjectionBase<TKey,T> where T : class,IReadModel<TKey>, new()
                                                                       where TKey : IEquatable<TKey>
    {
        public Instant Date { get; set; }
        public IProjection<TKey,T> Projection { get; set; }

        public HistoricalProjection(IStateWriter stateWriter, IEventStoreConnection connection) : base(stateWriter, connection)
        {
        }

        public override void When(IEvent e)
        {
            if(e.Timestamp <= Date)
                Projection.When(State, e);
        }
    }
}