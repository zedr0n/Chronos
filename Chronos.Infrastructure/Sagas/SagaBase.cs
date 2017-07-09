using System;
using System.Collections.Generic;
using Chronos.Infrastructure.Events;

namespace Chronos.Infrastructure.Sagas
{
    public abstract class SagaBase : ISaga, IConsumer
    {
        public Guid SagaId { get; }
        public int Version { get; private set; }

        protected abstract bool IsComplete();

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

        protected void OnComplete()
        {
            _uncommitedEvents.Add(new SagaCompleted { SourceId = SagaId } );
        }

        /// <summary>
        /// If the saga is in completed state there's no need to dispatch messages anymore
        /// </summary>
        protected void OnCompletion()
        {
            ClearUndispatchedMessages();
        }

        private void LoadFrom(IEnumerable<IEvent> pastEvents)
        {
            Version = 0;
            foreach (var e in pastEvents)
            {
                if (this.Dispatch(e))
                    Version++;
            }
            // as the events for sagas flow through event bus reloading saga will recommit events again
            _uncommitedEvents.Clear();
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

        protected bool When(IEvent e)
        {
            if (e.SourceId == SagaId && !IsComplete())
            {
                _uncommitedEvents.Add(e);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Add the events and commands to the dispatch queue
        /// </summary>
        /// <param name="m"></param>
        protected void SendMessage(IMessage m)
        {
            _undispatchedMessages.Add(m);
        }
    }
}