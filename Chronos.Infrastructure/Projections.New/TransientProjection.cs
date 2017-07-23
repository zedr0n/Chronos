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
                : base(projection) { }

            public T State { get; } = new T();

            protected override void When(IEvent e)
            {
                base.When(e);
                State.When(e);
            }
        }

        public ITransientProjection<T> Transient() => new TransientProjection(this);

    }
}