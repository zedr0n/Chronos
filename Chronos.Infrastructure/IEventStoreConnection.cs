using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using Chronos.Infrastructure.Interfaces;
using Chronos.Infrastructure.Projections.New;

namespace Chronos.Infrastructure
{
    public interface IEventStore
    {
        IObservable<IEvent> Events { get; }
        IObservable<IEvent> Alerts { get; }

        //IObservable<IGroupedObservable<StreamDetails, IEvent>> GetStreams(Func<StreamDetails, int> versionFunc);
        IObservable<IGroupedObservable<StreamDetails, IEvent>> GetEvents(IObservable<StreamRequest> requests);

        void Alert(IEvent e);
        
        IEventStoreConnection Connection { get; }
        IObservable<StreamDetails> GetLiveStreams();
    }

    public interface IEventStoreConnection
    {
        IEnumerable<StreamDetails> GetStreams();
        IObservable<Envelope> Events { get; }

        /// <summary>
        /// Appends events to the stream with the given details
        /// </summary>
        /// <param name="streamDetails">Details of the stream to append to</param>
        /// <param name="expectedVersion"></param>
        /// <param name="enumerable"></param>
        /// <exception cref="System.InvalidOperationException">If the stream is not at expected version</exception>
        void AppendToStream(StreamDetails streamDetails, int expectedVersion, IEnumerable<IEvent> enumerable);
        /// <summary>
        /// Read specified number events from the stream forward from starting position
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="start"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        IEnumerable<IEvent> ReadStreamEventsForward(StreamDetails stream, long start, int count);


    }
}