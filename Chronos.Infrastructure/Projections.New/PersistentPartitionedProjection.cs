using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reflection;
using Chronos.Infrastructure.Events;
using Chronos.Infrastructure.Interfaces;
using NodaTime;

namespace Chronos.Infrastructure.Projections.New
{
    public abstract class PersistentPartitionedProjection<T> : PersistentProjection<Guid,T>
        where T : class, IReadModel, new()
    {
        public PersistentPartitionedProjection(IEventStore eventStore, IStateWriter writer, IReadRepository readRepository) 
            : base(eventStore, writer, readRepository)
        {
            Key = new KeySelector(s => s.Key);
        }
    }
}