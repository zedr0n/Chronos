using System;
using System.Collections.Generic;
using Chronos.Infrastructure.Interfaces;

namespace Chronos.Infrastructure.Projections
{
    public class AggregateProjection<TKey,T> : PersistentProjection<TKey,T> 
        where TKey : IEquatable<TKey> where T : class, IReadModel, new()
    {
        
        internal AggregateProjection(IEventStore eventStore, IStateWriter writer, IReadRepository readRepository) : base(eventStore, writer, readRepository)
        {
        }

        protected override void Write(IEnumerable<TKey> keys, IList<IEvent> events)
        {
            base.Write(keys,events);
        }
        
        protected override void Write(TKey key, IList<IEvent> events)
        {
            base.Write(key, events);
        }
    }
}