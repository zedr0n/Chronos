using System;
using System.Collections;
using System.Collections.Generic;
using Chronos.Infrastructure.Events;

namespace Chronos.Infrastructure.Sagas
{
    public abstract class SagaBase : ISaga, IConsumer
    {
        public Guid SagaId { get; }

        protected readonly List<IEvent> PendingEvents = new List<IEvent>();
        protected readonly List<IEvent> UndispatchedMessages = new List<IEvent>();

        protected SagaBase(Guid sagaId)
        {
            SagaId = sagaId;
        }

        protected SagaBase(Guid id, IEnumerable<IEvent> pastEvents)
            : this(id)
        {
            LoadFrom(pastEvents);
        }

        private void LoadFrom(IEnumerable<IEvent> pastEvents)
        {
            foreach (var e in pastEvents)
            {
                this.Dispatch(e);
            }
        }

        public IEnumerable<IEvent> GetUncommittedEvents()
        {
            return PendingEvents;
        }

        public void ClearUncommittedEvents()
        {
            PendingEvents.Clear();
        }

        public IEnumerable<IEvent> GetUndispatchedMessages()
        {
            return UndispatchedMessages;
        }

        public void ClearUndispatchedMessages()
        {
            UndispatchedMessages.Clear();
        }
    }
}