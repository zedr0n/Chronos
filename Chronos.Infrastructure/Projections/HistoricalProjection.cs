using System;
using System.Collections.Generic;
using NodaTime;

namespace Chronos.Infrastructure.Projections
{
    public class HistoricalKey<TKey> : IEquatable<HistoricalKey<TKey>>
    {
        public TKey Key { get; set; }
        public Instant AsOf { get; set; }

        public bool Equals(HistoricalKey<TKey> other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return EqualityComparer<TKey>.Default.Equals(Key, other.Key) && AsOf.Equals(other.AsOf);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((HistoricalKey<TKey>) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (EqualityComparer<TKey>.Default.GetHashCode(Key) * 397) ^ AsOf.GetHashCode();
            }
        }
    }

    public class HistoricalProjection<TKey,T> : IProjection<HistoricalKey<TKey>> where T : class,IProjection<TKey>, new()
    {
        public HistoricalKey<TKey> Key { get; set; }
        public T Projection { get; }
        public int LastEvent { get; set; }

        public HistoricalProjection(TKey key, Instant asOf)
        {
            Projection = new T { Key = key };
            Key = new HistoricalKey<TKey> { Key = key, AsOf = asOf };
        }
    }
}