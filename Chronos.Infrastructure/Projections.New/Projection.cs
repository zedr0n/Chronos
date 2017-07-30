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

        private readonly List<StreamDetails> _streams = new List<StreamDetails>();

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

        public IProjection<T> From<TAggregate>() where TAggregate : IAggregate
        {
            _streams.AddRange(_connection.GetStreams<TAggregate>());
            return this;
        }

        public IProjection<T> From<TAggregate>(Guid id) where TAggregate : IAggregate
        {
            _streams.Add(new StreamDetails(typeof(TAggregate),id));
            return this;
        }

        protected virtual void When(IEvent e) { }
        protected virtual void When(IEvent e, StreamDetails stream) => When(e);

        public void Start()
        {
            foreach (var s in _streams)
                _connection.Subscriptions.SubscribeToStream(s.Name, -1, When);
        }
    }
}