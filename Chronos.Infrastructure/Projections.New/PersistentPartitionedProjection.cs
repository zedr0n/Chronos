using System;

namespace Chronos.Infrastructure.Projections.New
{
    public class PersistentPartitionedProjection<T> : PersistentProjection<Guid,T>
        where T : class, IReadModel, new()
    {
        internal PersistentPartitionedProjection(IEventStoreSubscriptions eventStore, IStateWriter writer)
            : base(eventStore, writer)
        {
            Key = stream => stream.Key;
        }
    }
}