using System;
using System.Collections.Generic;
using Chronos.Infrastructure.Commands;
using Chronos.Infrastructure.Events;

namespace Chronos.Infrastructure.Sagas
{
    public abstract class SagaBase : ISaga, IConsumer
    {
        public Guid SagaId { get; }
        public int Version { get; private set; }

        private readonly List<IEvent> _uncommitedEvents = new List<IEvent>();
        private readonly List<IMessage> _undispatchedMessages = new List<IMessage>();
        public IEnumerable<IEvent> UncommitedEvents => _uncommitedEvents;
        public IEnumerable<IMessage> UndispatchedMessages => _undispatchedMessages;

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
            Version = 0;
            foreach (var e in pastEvents)
            {
                if (this.Dispatch(e))
                    Version++;
            }
        }

        public void ClearUncommittedEvents()
        {
            Version += _uncommitedEvents.Count;
            _uncommitedEvents.Clear();
        }

        public void ClearUndispatchedMessages()
        {
            _undispatchedMessages.Clear();
        }

        protected void When(IEvent e)
        {
            _uncommitedEvents.Add(e);
        }

        protected void SendMessage(IMessage m)
        {
            _undispatchedMessages.Add(m);
        }
    }
}