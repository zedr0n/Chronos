using System.Collections.Generic;
using Chronos.Infrastructure.Events;

namespace Chronos.Infrastructure
{
    public interface IEventStoreConnection
    {
        void AppendToStream(string streamName, int expectedVersion, IEnumerable<IEvent> events);
        IEnumerable<IEvent> ReadStreamEventsForward(string streamName, long start, int count);
        IEnumerable<IEvent> GetAllEvents();
    }
}