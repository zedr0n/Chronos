using System;
using System.Collections.Generic;
using Chronos.Infrastructure.Events;

namespace Chronos.Infrastructure.Sagas
{
    public interface ISaga
    {
        Guid SagaId { get; }

        IEnumerable<IEvent> GetUncommittedEvents();
        void ClearUncommittedEvents();

        IEnumerable<IEvent> GetUndispatchedMessages();
        void ClearUndispatchedMessages();
    }
}