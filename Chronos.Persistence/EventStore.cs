using System;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using Chronos.Infrastructure;
using Chronos.Infrastructure.Interfaces;
using Chronos.Infrastructure.Logging;
using Chronos.Infrastructure.Projections.New;

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
            // new streams are always in current timeline
            //.Where(x => !x.IsBranch || x.Timeline == _timeline.TimelineId);

            // return streams.Where(x => !x.IsBranch);
            /*return streams.GroupBy(s => s.Name).Select(x => new GroupedObservable<string, StreamDetails>
            {
                Key = x.Key,
                Observable = x.GroupBy(s => s.Timeline).Switch()
            }).SelectMany(x => x.Observable);*/
            //Observable.Switch(streams.Where(x => !x.IsBranch),streams.Where(x => x.IsBranch))

        }

        public IObservable<IGroupedObservable<StreamDetails, IEvent>> GetEvents(IObservable<StreamRequest> requests)
        {
            var events = requests.SelectMany(r =>
            {
                var pastEvents = Connection.ReadStreamEventsForward(r.Stream, r.Version, int.MaxValue)
                    .OrderBy(e => e.Version);
                return pastEvents.Select(e => new Envelope(e,r.Stream)).ToObservable()
                    .Concat(_events.Where(e => e.Stream.Name == r.Stream.Name && e.Stream.Timeline == r.Stream.Timeline));
            });
            return events.GroupBy(x => x.Stream, x => x.Event);
        }

        public EventStore(IEventStoreConnection connection, ITimeline timeline, IDebugLog debugLog)
        {
            Connection = connection;
            Timeline = timeline;
            _debugLog = debugLog;
            _events = Connection.Events;
            
            //LiveStreams = Connection.GetLiveStreams().ToObservable()
            //    .Concat(_events.Select(x => x.Stream))
                //.GroupBy(x => new {x.Name, x.Timeline})
                //.Aggregate((acc, currentValue) => )
                //.SelectMany(x => x.MaxBy(s => s.Version))
                //.Select(x => x.Last());
                //.Select(x => x.)
            //    .Where(x => !x.Name.Contains("Saga"))
             //   .Where(x => x.Timeline == _timeline.TimelineId)
            //    .Distinct(x => new { x.Name, x.Timeline });

        }

        public void Alert(IEvent e)
        {
            _debugLog.WriteLine("Alert -> " + e.GetType().Name + " on thread " + Thread.CurrentThread.ManagedThreadId
                                + " with event store " + GetHashCode());
            _alerts.OnNext(e);
        }

    }
}