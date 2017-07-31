using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Chronos.Infrastructure;
using Chronos.Infrastructure.Interfaces;

namespace Chronos.Persistence
{
    public partial class SqlStoreConnection
    {
        private class EventStoreSubscriptions : IEventStoreSubscriptions
        {
            public delegate void EventAppendedHandler(object sender, EventAppendedArgs e);
            public event EventAppendedHandler EventAppended;

            private readonly ReplaySubject<StreamDetails> _streams;
            public IObservable<StreamDetails> Streams { get; }

            private readonly Dictionary<Action<StreamDetails,IEvent>, EventAppendedHandler> _eventAppended = new Dictionary<Action<StreamDetails,IEvent>, EventAppendedHandler>();
            private readonly SqlStoreConnection _connection;

            internal EventStoreSubscriptions(SqlStoreConnection connection)
            {
                _connection = connection;
                _streams = new ReplaySubject<StreamDetails>();

                Streams = _connection.GetStreams().ToObservable().Concat(_streams);
            }

            public void SubscribeToStream(StreamDetails stream, int eventNumber, Action<StreamDetails,IEvent> action)
            {
                var now = _connection._timeline.Now();
                var events = _connection.ReadStreamEventsForward(stream.Name, eventNumber, int.MaxValue)
                    .Where(e => e.Timestamp <= now)
                    .ToList().OrderBy(e => e.Timestamp);

                foreach (var e in events)
                    action(stream,e);

                _eventAppended[action] = (o, e) =>
                {
                    if (e.Stream.Name == stream.Name && _connection._timeline.Now() >= e.Event.Timestamp)
                        action(e.Stream,e.Event);
                };

                EventAppended += _eventAppended[action];
            }

            public void DropSubscription(StreamDetails stream, Action<StreamDetails,IEvent> action)
            {
                EventAppended -= _eventAppended[action];
            }

            internal void OnEventAppended(StreamDetails stream,IEvent e)
            {
                EventAppended?.Invoke(null, new EventAppendedArgs(stream, e));
            }

            internal void OnStreamAdded(StreamDetails details)
            {
                _streams.OnNext(details);
                //StreamAdded?.Invoke(null, new StreamAddedArgs(details));
            }

        }
    }
}