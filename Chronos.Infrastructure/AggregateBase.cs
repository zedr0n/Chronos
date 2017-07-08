using System;
using System.Collections.Generic;
using Chronos.Infrastructure.Events;

namespace Chronos.Infrastructure
{
    public abstract class AggregateBase : IAggregate
    {
        private readonly List<IEvent> _uncommitedEvents = new List<IEvent>();

        public Guid Id { get; }
        public int Version { get; private set; }
        public IEnumerable<IEvent> UncommitedEvents => _uncommitedEvents;

        public void ClearUncommitedEvents()
        {
            Version += _uncommitedEvents.Count;
            _uncommitedEvents.Clear();
        } 

        protected AggregateBase(Guid id)
        {
            Id = id;
        }

        protected AggregateBase(Guid id, IEnumerable<IEvent> pastEvents)
            :this(id)
        {
            LoadFrom(pastEvents);
        }

        private void LoadFrom(IEnumerable<IEvent> pastEvents)
        {
            Version = 0;
            foreach (var e in pastEvents)
            {
                if (this.Dispatch(e))
                    Version++;
            }
        }

        protected void RaiseEvent(IEvent e)
        {
            if (this.Dispatch(e))
                _uncommitedEvents.Add(e);
        }        
    }
}