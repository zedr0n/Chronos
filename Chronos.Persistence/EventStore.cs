using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using Chronos.Infrastructure;
using Chronos.Infrastructure.Interfaces;
using Chronos.Infrastructure.Logging;
using Chronos.Infrastructure.Projections;

namespace Chronos.Persistence
{
    public class EventStore : IEventStore,IEventBus
    {
        private readonly IObservable<Envelope> _events; 
        private readonly Subject<IEvent> _alerts = new Subject<IEvent>();

        public ITimeline Timeline { get; }

        private readonly IDebugLog _debugLog;

        public IEventStoreConnection Connection { get; }
        public IObservable<IEvent> Events => _events.Where(e => !e.SagaEvent).Select(e => e.Event);
        public IObservable<IEvent> Alerts => _alerts.AsObservable();

        public IObservable<StreamDetails> GetLiveStreams()
        {
            var streams = Connection.GetStreams()
                .Where(s => s.Timeline == Guid.Empty || s.Timeline == Timeline.TimelineId)
                .GroupBy(x => x.Name)
                .SelectMany(x => x.OrderBy(s => s.IsBranch).Take(1));

            return streams.ToObservable()
                .Concat(_events.Select(x => x.Stream))
                .Distinct(x => new {x.Name, x.Timeline});
        }

        public IObservable<IGroupedObservable<StreamDetails, IList<IEvent>>> GetEventsBuffered(
            IObservable<StreamRequest> requests)
        {
            var events = requests.SelectMany(r =>
            {
                var pastEvents = Connection.ReadStreamEventsForward(r.Stream, r.Version, int.MaxValue)
                    .OrderBy(e => e.Version)
                    .ToList();

                var lastEvent = pastEvents.LastOrDefault()?.Version ?? -1;
                // if we are saving multiple events at the same time
                // the events are going to be duplicated in stream past
                // and current stream events
                // we only need to handle the event once
                // => Distinct()
                return Observable.Return(new StreamBatch(pastEvents, r.Stream))
                    .Concat(_events.Where(e => e.Stream.Name == r.Stream.Name
                                               && e.Stream.Timeline == r.Stream.Timeline
                                               && e.Event.Version > lastEvent)
                        .Select(e => new StreamBatch(new List<IEvent> {e.Event}, r.Stream)));
                //.Distinct(x => x.Events.FirstOrDefault()?.EventNumber);

                //return pastEvents.Select(e => new Envelope(e,r.Stream)).ToObservable()
                //    .Concat(_events.Where(e => e.Stream.Name == r.Stream.Name && e.Stream.Timeline == r.Stream.Timeline))
                //    .Distinct(e => e.Event.EventNumber);
            });
            //return null;
            return events.GroupBy(x => x.Stream, x => x.Events);
            //return events.GroupBy(x => x.Stream, x => x.Select(l => )); 
        }
        
        public IObservable<IGroupedObservable<StreamDetails, IList<IEvent>>> GetEventsBuffered(
            IObservable<StreamDetails> streams)
        {
            var events = streams.SelectMany(s =>
            {
                var pastEvents = Connection.ReadStreamEventsForward(s, s.Version, int.MaxValue)
                    .OrderBy(e => e.Version)
                    .ToList();
                
                var lastEvent = pastEvents.LastOrDefault()?.Version ?? -1;
                return Observable.Return(new StreamBatch(pastEvents, s))
                    .Concat(_events.Where(e => e.Stream.Name == s.Name
                                               && e.Stream.Timeline == s.Timeline
                                               && e.Event.Version > lastEvent)
                        .Select(e => new StreamBatch(new List<IEvent> {e.Event}, s)));
            });
            return events.GroupBy(x => x.Stream, x => x.Events);
        }
        
        public IObservable<IGroupedObservable<StreamDetails, IEvent>> GetEvents(IObservable<StreamDetails> streams)
        {
            var lastEvents = new Dictionary<string, int>();
            var allPastEvents = streams.SelectMany(s =>
            {
                var pastEvents = Connection.ReadStreamEventsForward(s, s.Version, int.MaxValue)
                    .OrderBy(e => e.Version).ToList();
                lastEvents[s.Name] = pastEvents.LastOrDefault()?.Version ?? -1;
                return Observable.Return(pastEvents.Select(e => new Envelope(e, s)).ToList());
            }).Scan(new List<Envelope>(), (list, e) => {
                list.AddRange(e);
                return list.OrderBy(x => x.Event.Timestamp).ToList();
            });

            var allFutureEvents = streams.SelectMany(s => _events.Where(e => e.Stream.Name == s.Name
                                          && e.Stream.Timeline == s.Timeline
                                          && e.Event.Version > lastEvents[s.Name]));

            return allPastEvents.Merge(allFutureEvents)
                .GroupBy(x => x.Stream, x => x.Event);
            
            var events = streams.SelectMany(s =>
            {
                var pastEvents = Connection.ReadStreamEventsForward(s, s.Version, int.MaxValue)
                    .OrderBy(e => e.Version).ToList();
                var lastEvent = pastEvents.LastOrDefault()?.Version ?? -1;

                return pastEvents.Select(e => new Envelope(e, s)).ToObservable()
                    .Concat(_events.Where(e => e.Stream.Name == s.Name 
                                            && e.Stream.Timeline == s.Timeline
                                            && e.Event.Version > lastEvent));
            });
            return events.GroupBy(x => x.Stream, x => x.Event);
        }
        
        public IObservable<IGroupedObservable<StreamDetails, IEvent>> GetEvents(IObservable<StreamRequest> requests)
        {
            var events = requests.SelectMany(r =>
            {
                var pastEvents = Connection.ReadStreamEventsForward(r.Stream, r.Version, int.MaxValue)
                    .OrderBy(e => e.Version);
                // if we are saving multiple events at the same time
                // the events are going to be duplicated in stream past
                // and current stream events
                // we only need to handle the event once
                // => Distinct()
                return pastEvents.Select(e => new Envelope(e,r.Stream)).ToObservable()
                    .Concat(_events.Where(e => e.Stream.Name == r.Stream.Name && e.Stream.Timeline == r.Stream.Timeline))
                    .Distinct(e => e.Event.EventNumber);
            });
            return events.GroupBy(x => x.Stream, x => x.Event);
        }

        public EventStore(IEventStoreConnection connection, ITimeline timeline, IDebugLog debugLog)
        {
            Connection = connection;
            Timeline = timeline;
            _debugLog = debugLog;
            _events = Connection.Events;
        }

        public void Alert(IEvent e)
        {
            _debugLog.WriteLine("Alert -> " + e.GetType().Name + " on thread " + Thread.CurrentThread.ManagedThreadId);
                                //+ " with event store " + GetHashCode());
            _alerts.OnNext(e);
        }

    }
}