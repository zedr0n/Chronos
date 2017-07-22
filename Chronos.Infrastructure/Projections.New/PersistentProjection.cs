using System;
using System.Collections.Generic;
using System.Linq;
using Chronos.Infrastructure.Interfaces;
using NodaTime;

namespace Chronos.Infrastructure.Projections.New
{
    public partial class Projection<T>
    {
        private class PersistentProjection<TKey> : Projection<T>, IPersistentProjection<T>
            where TKey : IEquatable<TKey>
        {
            private readonly Func<string, TKey> _map;

            private PersistentProjection(Projection<T> projection)
                : base(projection) { }

            public PersistentProjection(Projection<T> projection, TKey key)
                : this(projection, s => key) { }

            public PersistentProjection(Projection<T> projection, Func<string,TKey> keymap)
                : this(projection)
            {
                _map = keymap;
            }

            protected override void When(IEvent e)
            {
                foreach (var key in _streams.Select(s => _map(s)).Distinct())
                    _writer.Write<TKey, T>(key, x => x.When(e));
            }

        }

        public IPersistentProjection<T> OutputState<TKey>(TKey key) where TKey : IEquatable<TKey>
        {
            return new PersistentProjection<TKey>(this, key);
        }

        public IPersistentProjection<T> OutputState<TKey>(Func<string, TKey> map) where TKey : IEquatable<TKey>
        {
            return new PersistentProjection<TKey>(this,map);
        }
    }

}