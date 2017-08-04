using System;
using Chronos.Infrastructure.Interfaces;

namespace Chronos.Infrastructure.Projections.New
{
    public class PersistentProjection<TKey,T> : Projection<T>, IPersistentProjection<T>
        where T : class, IReadModel, new()
        where TKey : IEquatable<TKey>
    {
        private readonly Func<StreamDetails, TKey> _key;
        private readonly IStateWriter _writer;

        internal PersistentProjection(IEventStoreSubscriptions eventStore, IStateWriter writer)
            : base(eventStore)
        {
            _writer = writer;
        }
        internal PersistentProjection(Projection<T> projection, Func<StreamDetails, TKey> key)
            : base(projection)
        {
            _key = key;
        }

        protected override void When(StreamDetails stream, IEvent e)
        {
            _writer.Write<TKey,T>(_key(stream),x => x.When(e));
            //Write(_key(stream),e);
            base.When(stream, e);
        }
    }

    public class PersistentPartitionedProjection<T> : PersistentProjection<Guid,T>, IPartitionedProjection<T>
        where T : class, IReadModel, new()
    {
        internal PersistentPartitionedProjection(IEventStoreSubscriptions eventStore, IStateWriter writer)
            : base(eventStore, writer)
        {
            
        }

        public PersistentPartitionedProjection(Projection<T> projection)
            : base(projection, stream => stream.Key) { }
    }
}