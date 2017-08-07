using System;
using System.Collections.Generic;
using Chronos.Infrastructure.Events;
using Chronos.Infrastructure.Interfaces;
using Chronos.Infrastructure.Logging;

namespace Chronos.Infrastructure.Sagas
{
    public abstract class SagaBase : ISaga
    {
        public IDebugLog DebugLog { get; set; }
        public Guid SagaId { get; private set; }
        public int Version { get; private set; }

        private readonly List<IEvent> _uncommitedEvents = new List<IEvent>();
        private readonly List<IMessage> _undispatchedMessages = new List<IMessage>();
        public IEnumerable<IEvent> UncommitedEvents => _uncommitedEvents;
        public IEnumerable<IMessage> UndispatchedMessages => _undispatchedMessages;

        protected SagaBase() { }
        protected SagaBase(Guid sagaId)
        {
            SagaId = sagaId;
        }

        public T LoadFrom<T>(Guid id,IEnumerable<IEvent> pastEvents) where T : class, ISaga,new()
        {
            SagaId = id;
            Version = 0;
            foreach (var e in pastEvents)
                this.Dispatch(e);

            return this as T;
        }

        public void ClearUncommittedEvents()
        {
            _uncommitedEvents.Clear();
        }

        public void ClearUndispatchedMessages()
        {
            _undispatchedMessages.Clear();
        }

        protected void When(IEvent e)
        {
            Version++;
            _uncommitedEvents.Add(e);
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