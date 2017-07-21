using System;
using System.Collections.Generic;
using Chronos.Infrastructure.Interfaces;

namespace Chronos.Infrastructure.Projections.New
{
    public partial class Projection<T> 
    {
        private class TransientProjection : Projection<T>, ITransientProjection<T>
        {
            public TransientProjection(Projection<T> projection)
                : base(projection._connection, projection._writer)
            {
                _handler = projection._handler;
                Streams = projection.Streams;
            }

            public T State { get; } = new T();

            public new ITransientProjection<T> From(IEnumerable<string> streams)
            {
                base.From(streams);
                return this;
            }

            public override IProjection<TKey,T> OutputState<TKey>(TKey key)
            {
                throw new InvalidOperationException("Transient projections don't output state");
            }

            protected override void When(IEvent e)
            {
                When(State, e);
            }
        }

    }
}