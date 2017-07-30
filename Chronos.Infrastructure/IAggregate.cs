using System;
using System.Collections.Generic;
using Chronos.Infrastructure.Events;
using Chronos.Infrastructure.Interfaces;

namespace Chronos.Infrastructure
{
    public interface IAggregate : IConsumer
    {
        int Version { get; }
        Guid Id { get; }

        IEnumerable<IEvent> UncommitedEvents { get; }
        void ClearUncommitedEvents();
        T LoadFrom<T>(Guid id, IEnumerable<IEvent> pastEvents) where T : class, IAggregate, new();

    }
}