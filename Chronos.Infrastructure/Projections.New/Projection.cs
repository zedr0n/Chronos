using System;
using System.Collections.Generic;
using Chronos.Infrastructure.Interfaces;
using NodaTime;

namespace Chronos.Infrastructure.Projections.New
{
    public partial class Projection<T> : IProjection<T> where T : class, IReadModel, new()
    {
        private readonly IEventStoreConnection _connection;
        private readonly IStateWriter _writer;
        private IProjectionHandler<T> _handler;

        private IEnumerable<string> Streams { get; set; } = new List<string>();

        public Projection(IEventStoreConnection connection, IStateWriter writer)
        {
            _connection = connection;
            _writer = writer;
        }

        public IProjection<T> Transient() => new TransientProjection(this);
        public ITransientProjection<T> AsOf(Instant date) => new HistoricalProjection(this,date);
        public virtual IProjection<TKey, T> OutputState<TKey>(TKey key) where TKey : IEquatable<TKey>
        {
            return new PersistentProjection<TKey, T>(this, key);
        }

        public IProjection<T> From(IEnumerable<string> streams)
        {
            Streams = streams;
            return this;
        }

        public IProjection<T> When(IProjectionHandler<T> handler)
        {
            _handler = handler;
            return this;
        }

        private void When(T state, IEvent e)
        {
            _handler?.When(state,e);
        }

        protected virtual void When(IEvent e) { }

        public void Start()
        {
            foreach (var s in Streams)
                _connection.SubscribeToStream(s, -1, When);
        }
    }
}