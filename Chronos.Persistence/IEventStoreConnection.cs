using System.Collections.Generic;

namespace Chronos.Persistence
{
    public interface IEventStoreConnection
    {
        void AppendToStream(string streamName, int expectedVersion, IEnumerable<Event> events);
        IEnumerable<Event> ReadStreamEventsForward(string streamName, long start, int count);
    }
}