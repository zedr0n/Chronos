using System;
using System.Collections.Generic;
using Chronos.Infrastructure.Interfaces;

namespace Chronos.Infrastructure
{
    public interface IEventStoreSubscriptions
    {
        void OnStreamAdded(Action<StreamDetails> action);
        void SubscribeToStream(StreamDetails stream, int eventNumber, Action<StreamDetails,IEvent> action);
        void DropSubscription(StreamDetails stream, Action<StreamDetails,IEvent> action);
    }

    public interface IEventStoreWriter
    {
        void AppendToNull(IEnumerable<IEvent> enumerable);

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
        /// <param name="streamName"></param>
        /// <param name="start"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        IEnumerable<IEvent> ReadStreamEventsForward(string streamName, long start, int count);
        IEnumerable<IEvent> GetAggregateEvents();
    }

    public interface IEventStoreConnection
    {
        IEventStoreSubscriptions Subscriptions { get; }
        IEventStoreWriter Writer { get; }
        IEventStoreReader Reader { get; }

        bool Exists(StreamDetails stream);

        void Initialise();

        IEnumerable<StreamDetails> GetStreams(Func<StreamDetails, bool> predicate);
        IEnumerable<StreamDetails> GetStreams<T>();

    }
}