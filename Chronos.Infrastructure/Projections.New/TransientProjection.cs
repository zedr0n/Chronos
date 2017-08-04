using Chronos.Infrastructure.Events;
using Chronos.Infrastructure.Interfaces;

namespace Chronos.Infrastructure.Projections.New
{
    public class TransientProjection<T> : Projection<T>, ITransientProjection<T>
        where T : class, IReadModel, new()
    {
        internal TransientProjection(IEventStoreSubscriptions eventStore)
            : base(eventStore) { }

        internal TransientProjection(Projection<T> projection)
            : base(projection) { }

        public T State { get; } = new T();

        protected override void When(StreamDetails stream, IEvent e)
        {
            State.When(e);
            base.When(stream, e);
        }
    }
}