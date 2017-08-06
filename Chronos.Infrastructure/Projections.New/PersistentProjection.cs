using System;
using Chronos.Infrastructure.Interfaces;

namespace Chronos.Infrastructure.Projections.New
{
    public class PersistentProjection<TKey,T> : Projection, IPersistentProjection<T>
        where T : class, IReadModel, new()
        where TKey : IEquatable<TKey>
    {
        protected Func<StreamDetails, TKey> Key { get; set; }
        private readonly IStateWriter _writer;

        internal PersistentProjection(IEventStoreSubscriptions eventStore, IStateWriter writer)
            : base(eventStore)
        {
            _writer = writer;
        }

        protected override void When(StreamDetails stream, IEvent e)
        {
            _writer.Write<TKey,T>(Key(stream),x => x.When(e));
            //Write(_key(stream),e);
            base.When(stream, e);
        }
    }

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