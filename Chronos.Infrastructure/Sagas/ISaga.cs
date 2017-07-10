using System;
using System.Collections.Generic;
using Chronos.Infrastructure.Events;
using Chronos.Infrastructure.Interfaces;

namespace Chronos.Infrastructure.Sagas
{
    public interface ISaga
    {
        Guid SagaId { get; }
        int Version { get; }

        IEnumerable<IEvent> UncommitedEvents { get; }
        void ClearUncommittedEvents();

        IEnumerable<IMessage> UndispatchedMessages { get; }
        void ClearUndispatchedMessages();
    }
}