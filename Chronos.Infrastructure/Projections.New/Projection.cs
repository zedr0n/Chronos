using System;
using Chronos.Infrastructure.Events;
using Chronos.Infrastructure.Interfaces;

namespace Chronos.Infrastructure.Projections.New
{

    public partial class Projection<T> : IProjectionFrom<T>, IProjection<T> where T : class, IReadModel, new()
    {
        private readonly IEventStoreConnection _connection;
        private readonly IStateWriter _writer;
        private readonly IEventBus _eventBus;

        private Func<StreamDetails,bool> _from = s => true;

        private Projection(Projection<T> projection)
            : this(projection._connection, projection._writer, projection._eventBus)
        {
            _from = projection._from;
        }

        public Projection(IEventStoreConnection connection, IStateWriter writer, IEventBus eventBus)
        {
            _connection = connection;
            _writer = writer;
            _eventBus = eventBus;
        }

        public IProjection<T> From<TAggregate>() where TAggregate : IAggregate
        {
            _from = s => s.SourceType == typeof(TAggregate).Name;
            return this;
        }

        public IProjection<T> From<TAggregate>(Guid id) where TAggregate : IAggregate
        {
            _from = s => s.SourceType == typeof(TAggregate).Name && s.Id == id;
            return this;
        }

        private void When(ReplayCompleted e)
        {
            Start();
        }
        protected virtual void When(IEvent e) { }
        protected virtual void When(StreamDetails stream, IEvent e) => When(e);

        private void Subscribe(StreamDetails stream)
        {
            _connection.Subscriptions.SubscribeToStream(stream, -1, When);
        }

        public void Start()
        {
            foreach (var s in _connection.GetStreams(_from))
                Subscribe(s);

            _connection.Subscriptions.OnStreamAdded(s =>
            {
                if (_from(s))
                    Subscribe(s);
            });

            _eventBus.Subscribe<ReplayCompleted>(When);
        }
    }
}