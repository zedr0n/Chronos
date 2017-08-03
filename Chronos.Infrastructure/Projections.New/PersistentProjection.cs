using System;
using Chronos.Infrastructure.Interfaces;

namespace Chronos.Infrastructure.Projections.New
{
    public class PersistentProjection<TKey,T> : Projection<T>, IPersistentProjection<T>
        where T : class, IReadModel, new()
        where TKey : IEquatable<TKey>
    {
        private readonly Func<StreamDetails, TKey> _key;

        internal PersistentProjection(Projection<T> projection, Func<StreamDetails, TKey> key)
            : base(projection)
        {
            _key = key;
        }

        protected override void When(StreamDetails stream, IEvent e)
        {
            Write(_key(stream),e);
            base.When(stream, e);
        }
    }

    public class PersistentPartitionedProjection<T> : PersistentProjection<Guid,T>, IPartitionedProjection<T>
        where T : class, IReadModel, new()
    {
        public PersistentPartitionedProjection(Projection<T> projection)
            : base(projection, stream => stream.Key) { }
    }
}