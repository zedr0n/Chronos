using System;
using System.Collections.Generic;
using System.Linq;
using Chronos.Infrastructure.Interfaces;
using NodaTime;

namespace Chronos.Infrastructure.Projections.New
{
    public partial class Projection<T>
    {
        private class PersistentProjection<TKey, T> : Projection<T>, IProjection<TKey, T> where T : class, IReadModel, new()
            where TKey : IEquatable<TKey>
        {
            private Func<string, TKey> _map;

            public PersistentProjection(Projection<T> projection, TKey key)
                : base(projection._connection,projection._writer)
            {
                Streams = projection.Streams;
                _handler = projection._handler;
                _map = s => key;
            }
            
            public new IProjection<TKey, T> When(IProjectionHandler<T> handler)
            {
                base.When(handler);
                return this;
            }

            public new IProjection<TKey, T> From(IEnumerable<string> streams)
            {
                base.From(streams);
                return this;
            }

            public IProjection<TKey, T> ForEachStream(Func<string, TKey> map)
            {
                _map = map;
                return this;
            }

            protected override void When(IEvent e)
            {
                foreach (var key in Streams.Select(s => _map(s)).Distinct())
                    _writer.Write<TKey, T>(key, x => When(x, e));
            }

        }
    }

}