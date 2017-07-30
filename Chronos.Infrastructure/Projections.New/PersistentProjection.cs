using System;
using Chronos.Infrastructure.Interfaces;

namespace Chronos.Infrastructure.Projections.New
{
    public partial class Projection<T>
    {
        private class PersistentProjection<TKey> : Projection<T>, IPersistentProjection<T>
            where TKey : IEquatable<TKey>
        {
            private readonly TKey _key;
            
            internal PersistentProjection(Projection<T> projection, TKey key)
                : base(projection)
            {
                _key = key;
            }

            protected override void When(IEvent e)
            {
                _writer.Write<TKey,T>(_key, x => x.When(e));
                base.When(e);
            }
        }

        private class PersistentPartitionedProjection : Projection<T>, IPartitionedProjection<T>, IPersistentProjection<T>
        {
            internal PersistentPartitionedProjection(Projection<T> projection) : base(projection)
            {
            }

            protected override void When(StreamDetails stream, IEvent e)
            {
                _writer.Write<Guid, T>(stream.Id, x => x.When(e));
                base.When(e);
            }

            public IPersistentProjection<T> OutputState() => this;
        }

        public IPersistentProjection<T> OutputState<TKey>(TKey key) where TKey : IEquatable<TKey>
        {
            return new PersistentProjection<TKey>(this,key);
        }

        public IPartitionedProjection<T> ForEachStream()
        {
            return new PersistentPartitionedProjection(this);
        }
    }

}