using System;
using System.Collections.Generic;
using Chronos.Infrastructure.Interfaces;

namespace Chronos.Infrastructure
{
    public interface IEventStoreSubscriptions
    {
        IObservable<IEvent> Events { get; }
        IObservable<IEvent> GetEvents(StreamDetails stream, int eventNumber);
        IObservable<StreamDetails> GetStreams();

        IObservable<IEvent> Alerts { get; }
        void Alert(IEvent e);
    }

    public interface IEventStoreWriter
    {
        /// <summary>
        /// Appends events to the stream with the given details
        /// </summary>
        /// <param name="streamDetails">Details of the stream to append to</param>
        /// <param name="expectedVersion"></param>
        /// <param name="enumerable"></param>
        /// <exception cref="System.InvalidOperationException">If the stream is not at expected version</exception>
        void AppendToStream(StreamDetails streamDetails, int expectedVersion, IEnumerable<IEvent> enumerable);
    }

    public interface IEventStoreReader
    {
        /// <summary>
        /// Read specified number events from the stream forward from starting position
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="start"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        IEnumerable<IEvent> ReadStreamEventsForward(StreamDetails stream, long start, int count);
    }

    public interface IEventStoreConnection
    {
        IEventStoreSubscriptions Subscriptions { get; }
        IEventStoreWriter Writer { get; }
        IEventStoreReader Reader { get; }

        bool Exists(StreamDetails stream);
    }
}