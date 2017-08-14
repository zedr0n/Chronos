using System;
using System.Collections.Generic;
using Chronos.Infrastructure.Interfaces;
using Chronos.Infrastructure.Logging;

namespace Chronos.Infrastructure
{
    public interface ISaga
    {
        IDebugLog DebugLog { get; set; }
        Guid SagaId { get; }
        int Version { get; }

        IEnumerable<IEvent> UncommitedEvents { get; }
        void ClearUncommittedEvents();

        IEnumerable<IMessage> UndispatchedMessages { get; }
        void ClearUndispatchedMessages();

        /// <summary>
        /// Hydrate the saga from events sequence
        /// </summary>
        /// <param name="id">Saga id</param>
        /// <param name="pastEvents">Past events sequence</param>
        /// <typeparam name="T">Saga type</typeparam>
        /// <returns>Hydrated saga instance</returns>
        T LoadFrom<T>(Guid id, IEnumerable<IEvent> pastEvents) where T : class,ISaga,new();
    }
}