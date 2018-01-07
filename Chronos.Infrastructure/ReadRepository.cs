using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Chronos.Infrastructure
{
    public class ReadRepository : IMemoryReadRepository
    {
        private class Id : IEquatable<Id>
        {
            public Type Type { get; }
            public Guid Timeline { get; }

            public Id(Type type, Guid timeline)
            {
                Type = type;
                Timeline = timeline;
            }

            public bool Equals(Id other)
            {
                if (ReferenceEquals(null, other)) return false;
                if (ReferenceEquals(this, other)) return true;
                return Type == other.Type && Timeline.Equals(other.Timeline);
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != this.GetType()) return false;
                return Equals((Id) obj);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    return ((Type != null ? Type.GetHashCode() : 0) * 397) ^ Timeline.GetHashCode();
                }
            }
        }

        private class Id<TKey> : Id, IEquatable<Id<TKey>> where TKey : IEquatable<TKey>
        {
            public TKey Key { get; }

            public Id(Type type, TKey key, Guid timeline)
                : base(type,timeline)
            {
                Key = key;
            }

            public bool Equals(Id<TKey> other)
            {
                if (ReferenceEquals(null, other)) return false;
                if (ReferenceEquals(this, other)) return true;
                return base.Equals(other) && EqualityComparer<TKey>.Default.Equals(Key, other.Key);
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != this.GetType()) return false;
                return Equals((Id<TKey>) obj);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    return (base.GetHashCode() * 397) ^ EqualityComparer<TKey>.Default.GetHashCode(Key);
                }
            }
        }
        
        private readonly ConcurrentDictionary<Id,IReadModel> _dictionary = new ConcurrentDictionary<Id, IReadModel>();
        
        private readonly ITimeline _timeline;

        public ReadRepository(ITimeline timeline)
        {
            _timeline = timeline;
        }

        public T GetOrAdd<TKey, T>(TKey key)
            where T : class, IReadModel, new()
            where TKey : IEquatable<TKey>
        {
            var id = new Id<TKey>(typeof(T),key,_timeline.TimelineId);
            return _dictionary.GetOrAdd(id,
                x =>
                {
                    var model = new T();
                    ((IReadModel<TKey>) model).Key = key;
                    return model;
                }) as T;
        }
        
        public T Find<TKey, T>(TKey key) where T : class,IReadModel
            where TKey: IEquatable<TKey>
        {
            var id = new Id<TKey>(typeof(T),key,_timeline.TimelineId);
            _dictionary.TryGetValue(id, out var readModel);
            return readModel as T;
        }

        public T Find<TKey, T, TProperty>(TKey key, Expression<Func<T, IEnumerable<TProperty>>> include) where TKey : IEquatable<TKey> where T : class, IReadModel where TProperty : class
        {
            return Find<TKey, T>(key);
        }

        public void Set<TKey, T>(TKey key, T readModel) where T : class, IReadModel
            where TKey : IEquatable<TKey>
        {
            var id = new Id<TKey>(typeof(T),key,_timeline.TimelineId);
            _dictionary.AddOrUpdate(id, readModel, (x, prev) =>
                readModel);
        }

        public T Find<T>(Func<T, bool> predicate) where T : class, IReadModel
        {
            lock (_dictionary)
            {
                return _dictionary.GroupBy(x => x.Key.Type)
                    .SingleOrDefault(x => x.Key == typeof(T))
                    ?.SelectMany(x => new List<T> { (T) x.Value })
                    .SingleOrDefault(x => x.Timeline == _timeline.TimelineId && predicate(x));
            }
        }
    }
}