using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Chronos.Infrastructure.Interfaces;

namespace Chronos.Infrastructure
{
    public abstract class AggregateBase : IAggregate
    {
        private readonly Dictionary<Type,Action<IEvent>> _handlers = new Dictionary<Type, Action<IEvent>>();
        private readonly List<IEvent> _uncommitedEvents = new List<IEvent>();

        public int Version { get; private set; }
        public Guid Id { get; private set; }
        public IEnumerable<IEvent> UncommitedEvents => _uncommitedEvents;

        public void ClearUncommitedEvents()
        {
            _uncommitedEvents.Clear();
        }

        protected AggregateBase()
        {
            foreach (var m in GetType().GetTypeInfo().GetDeclaredMethods("When"))
            {
                _handlers.Add(m.GetParameters().Single().ParameterType,e => m.Invoke(this,new object[] { e }));
            }
        }

        protected AggregateBase(Guid id)
            : this()
        {
            Id = id;
        }

        public T LoadFrom<T>(Guid id,IEnumerable<IEvent> pastEvents) where T : class,IAggregate,new()
        {
            Id = id;
            Version = 0;
            foreach (var e in pastEvents)
            {
                if (Dispatch(e))
                    Version++;
            }
            return this as T;
        }

        private bool Dispatch(IEvent e)
        {
            if (_handlers.ContainsKey(e.GetType()))
            {
                _handlers[e.GetType()].Invoke(e);
                return true;
            }
            return false;
        }

        protected void RaiseEvent(IEvent e)
        {
            if (Dispatch(e))
            {
                Version++;
                _uncommitedEvents.Add(e);
            }
        }        
    }
}