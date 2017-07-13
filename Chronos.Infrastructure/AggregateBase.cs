using System;
using System.Collections.Generic;
using Chronos.Infrastructure.Events;

namespace Chronos.Infrastructure
{
    public abstract class AggregateBase : IAggregate
    {
        private readonly List<IEvent> _uncommitedEvents = new List<IEvent>();

        public Guid Id { get; private set; }
        public int Version { get; private set; }
        public IEnumerable<IEvent> UncommitedEvents => _uncommitedEvents;

        public void ClearUncommitedEvents()
        {
            _uncommitedEvents.Clear();
        } 

        protected AggregateBase() { }

        protected AggregateBase(Guid id)
        {
            Id = id;
        }

        public T LoadFrom<T>(Guid id,IEnumerable<IEvent> pastEvents) where T : class,IAggregate,new()
        {
            Id = id;
            Version = 0;
            foreach (var e in pastEvents)
            {
                if (this.Dispatch(e))
                    Version++;
            }
            return this as T;
        }

        protected void RaiseEvent(IEvent e)
        {
            if (this.Dispatch(e))
            {
                Version++;
                _uncommitedEvents.Add(e);
            }
        }        
    }
}