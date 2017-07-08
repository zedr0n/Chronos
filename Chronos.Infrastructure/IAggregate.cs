﻿using System;
using System.Collections.Generic;
using Chronos.Infrastructure.Events;

namespace Chronos.Infrastructure
{
    public interface IAggregate : IConsumer
    {
        Guid Id { get; }
        int Version { get; }
        IEnumerable<IEvent> UncommitedEvents { get; }
        void ClearUncommitedEvents();
    }

}