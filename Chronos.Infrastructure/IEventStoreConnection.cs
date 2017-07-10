using System;
using System.Collections.Generic;
using Chronos.Infrastructure.Events;

namespace Chronos.Infrastructure
{
    public interface IEventStoreConnection
    {
        void AppendToNull(IEnumerable<IEvent> enumerable);
        /// <summary>
        /// Appends events to the stream with the given name
        /// </summary>
        /// <param name="streamName">Name of the stream to read from</param>
        /// <param name="expectedVersion"></param>
        /// <param name="enumerable"></param>
        /// <exception cref="System.InvalidOperationException">If the stream is not at expected version</exception>
        void AppendToStream(string streamName, int expectedVersion, IEnumerable<IEvent> enumerable);
        /// <summary>
        /// Read specified number events from the stream forward from starting position
        /// </summary>
        /// <param name="streamName"></param>
        /// <param name="start"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        IEnumerable<IEvent> ReadStreamEventsForward(string streamName, long start, int count);

        void Initialise();

        /// <summary>
        /// Aggregate and return events from all streams
        /// </summary>
        /// <returns></returns>
        IEnumerable<IEvent> GetAllEvents();

        IEnumerable<IEvent> GetAggregateEvents();
    }
}