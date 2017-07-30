using System;
using System.Collections.Generic;
using System.Linq;
using Chronos.Infrastructure;
using Chronos.Infrastructure.Interfaces;

namespace Chronos.Persistence
{
    public partial class SqlStoreConnection
    {
        private class EventStoreSubscriptions : IEventStoreSubscriptions
        {
            public delegate void EventAppendedHandler(object sender, EventAppendedArgs e);
            public delegate void StreamAddedHandler(object sender, StreamAddedArgs e);
            public event EventAppendedHandler EventAppended;
            public event StreamAddedHandler StreamAdded;

            private readonly Dictionary<Action<StreamDetails,IEvent>, EventAppendedHandler> _eventAppended = new Dictionary<Action<StreamDetails,IEvent>, EventAppendedHandler>();
            private readonly Dictionary<Action<StreamDetails>, StreamAddedHandler> _streamAdded = new Dictionary<Action<StreamDetails>, StreamAddedHandler>();
            private readonly SqlStoreConnection _connection;

            public EventStoreSubscriptions(SqlStoreConnection connection)
            {
                _connection = connection;
            }

            public void OnStreamAdded(Action<StreamDetails> action)
            {
                _streamAdded[action] = (o, e) =>
                {
                    action(e.Details);
                };

                StreamAdded += _streamAdded[action];
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

            public void OnEventAppended(StreamDetails stream,IEvent e)
            {
                EventAppended?.Invoke(null, new EventAppendedArgs(stream, e));
            }

            public void OnStreamAdded(StreamDetails details)
            {
                StreamAdded?.Invoke(null, new StreamAddedArgs(details));
            }

        }
    }
}