using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Chronos.Infrastructure.ProjectionServices
{
    public abstract class PersistentKeySelector<TKey,T> : IKeySelector<TKey,T> where T : IReadModel
    {
        private readonly ConcurrentDictionary<TKey, bool> _keys = new ConcurrentDictionary<TKey, bool>();

        protected PersistentKeySelector(IStreamSelector<T> streamSelector)
        {
            streamSelector.Streams.Subscribe(s => _keys.TryAdd(Key(s),true));
        }

        protected abstract TKey Key(StreamDetails s);
        public virtual IEnumerable<TKey> Get(StreamDetails s)
        {
            return new List<TKey> { Key(s) };
        }
    }
}