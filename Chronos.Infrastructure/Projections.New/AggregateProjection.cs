using System;
using System.Reactive.Linq;

namespace Chronos.Infrastructure.Projections.New
{
    public class AggregateProjection<T> : Projection<T> where T : class, IReadModel, new()
    {
        internal AggregateProjection(Projection<T> projection, Guid id) : base(projection)
        {
            Streams = projection.Streams.Where(s => s.Key == id);
        }
    }
}