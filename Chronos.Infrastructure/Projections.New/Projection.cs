using System;
using System.Collections.Generic;
using System.Linq;
using Chronos.Infrastructure.Interfaces;
using NodaTime;

namespace Chronos.Infrastructure.Projections.New
{
    public partial class Projection<T> : IProjectionFrom<T>, IProjection<T> where T : class, IReadModel, new()
    {
        private readonly IEventStoreConnection _connection;
        private readonly IStateWriter _writer;

        private List<string> _streams = new List<string>();

        private Projection(Projection<T> projection)
            : this(projection._connection, projection._writer)
        {
            _streams = projection._streams;
        }

        public Projection(IEventStoreConnection connection, IStateWriter writer)
        {
            _connection = connection;
            _writer = writer;
        }

        public IProjection<T> From(string streamName)
        {
            _streams.Add(streamName);
            return this;
        }

        public IProjection<T> From(IEnumerable<string> streams)
        {
            _streams = streams.ToList();
            return this;
        }

        public IProjection<T> From<TAggregate>() where TAggregate : IAggregate
        {
            return From(_connection.GetStreams<TAggregate>());
        }

        public IProjection<T> From<TKey, TAggregate>(TKey key) where TAggregate : IAggregate
        {
            return From(StreamExtensions.StreamName<TKey, TAggregate>(key));
        }

        protected virtual void When(IEvent e) { }

        public void Start()
        {
            foreach (var s in _streams)
                _connection.Subscriptions.SubscribeToStream(s, -1, When);
        }
    }
}