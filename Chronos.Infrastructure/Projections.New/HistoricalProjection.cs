using System;
using Chronos.Infrastructure.Interfaces;
using NodaTime;

namespace Chronos.Infrastructure.Projections.New
{
    public partial class Projection<T>
    {
        private class HistoricalProjection : TransientProjection
        {
            private readonly Instant _date;

            public HistoricalProjection(Projection<T> projection, Instant date) 
                : base(projection)
            {
                _date = date;
            }

            protected override void When(IEvent e)
            {
                if (e.Timestamp <= _date)
                    base.When(e);
            }
        }
    }

}