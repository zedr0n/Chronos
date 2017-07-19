using System;
using System.Collections.Generic;
using NodaTime;

namespace Chronos.Infrastructure.Projections
{
    public class HistoricalKey<TKey> : IEquatable<HistoricalKey<TKey>> 
    {
        private readonly TKey _key;
        private readonly Instant _asOf;

        public HistoricalKey(TKey key, Instant asOf)
        {
            _key = key;
            _asOf = asOf;
        }

        public static bool operator == (HistoricalKey<TKey> left, HistoricalKey<TKey> right)
        {
            return ReferenceEquals(null,left) ? ReferenceEquals(null,right) : left.Equals(right);
        }

        public static bool operator !=(HistoricalKey<TKey> left, HistoricalKey<TKey> right)
        {
            return !(left == right);
        }


        public bool Equals(HistoricalKey<TKey> other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return _key.Equals(other._key) && _asOf == other._asOf;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((HistoricalKey<TKey>) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (EqualityComparer<TKey>.Default.GetHashCode(_key) * 397) ^ _asOf.GetHashCode();
            }
        }
    }

    public class HistoricalProjection<TKey,T> : ProjectionBase<HistoricalKey<TKey>> 
                                                    where T : class,IProjection<TKey>, new()
    {
        public T Projection { get; }

        public HistoricalProjection()
        {
            Projection = new T();
        }
    }
}