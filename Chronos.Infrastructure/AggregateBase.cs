using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Chronos.Infrastructure.Interfaces;

namespace Chronos.Infrastructure
{
    public abstract class AggregateBase : IAggregate
    {        private readonly List<IEvent> _uncommitedEvents = new List<IEvent>();

        public int Version { get; private set; }
        public Guid Id { get; protected set; }
        public IEnumerable<IEvent> UncommitedEvents => _uncommitedEvents;

        public void ClearUncommitedEvents()
        {
            _uncommitedEvents.Clear();
        }

        protected virtual void When(IEvent e)
        {
            Version++;
            _uncommitedEvents.Add(e);
        }

        public T LoadFrom<T>(Guid id,IEnumerable<IEvent> pastEvents) where T : class,IAggregate,new()
        {
            Id = id;
            //Version = 0;
            foreach (var e in pastEvents)
                When(e);

            ClearUncommitedEvents();
            return this as T;
        }
    }
}