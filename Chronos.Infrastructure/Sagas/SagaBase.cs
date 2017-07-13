using System;
using System.Collections.Generic;
using Chronos.Infrastructure.Events;
using Chronos.Infrastructure.Interfaces;

namespace Chronos.Infrastructure.Sagas
{
    public abstract class SagaBase : ISaga, IConsumer<SagaCompleted>
    {
        public Guid SagaId { get; private set; }
        public int Version { get; private set; }

        protected abstract bool IsComplete();

        private readonly List<IEvent> _uncommitedEvents = new List<IEvent>();
        private readonly List<IMessage> _undispatchedMessages = new List<IMessage>();
        public IEnumerable<IEvent> UncommitedEvents => _uncommitedEvents;
        public IEnumerable<IMessage> UndispatchedMessages => _undispatchedMessages;


        protected SagaBase() { }
        protected SagaBase(Guid sagaId)
        {
            SagaId = sagaId;
        }

        protected void OnComplete()
        {
            _uncommitedEvents.Add(new SagaCompleted { SourceId = SagaId } );
        }

        /// <summary>
        /// If the saga is in completed state there's no need to dispatch messages anymore
        /// </summary>
        public void When(SagaCompleted e)
        {
            ClearUndispatchedMessages();
        }

        public T LoadFrom<T>(Guid id,IEnumerable<IEvent> pastEvents) where T : class, ISaga,new()
        {
            SagaId = id;
            Version = 0;
            foreach (var e in pastEvents)
            {
                if (this.Dispatch(e))
                    Version++;
            }
            // as the events for sagas flow through event bus reloading saga will recommit events again
            _uncommitedEvents.Clear();

            return this as T;
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
            if (!IsComplete())
            {
                if(!e.Replaying)
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