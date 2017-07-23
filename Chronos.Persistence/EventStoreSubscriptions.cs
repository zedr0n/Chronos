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

            private readonly Dictionary<Action<IEvent>, EventAppendedHandler> _eventAppended = new Dictionary<Action<IEvent>, EventAppendedHandler>();
            private readonly Dictionary<Action<IEvent>, StreamAddedHandler> _streamAdded = new Dictionary<Action<IEvent>, StreamAddedHandler>();
            private readonly SqlStoreConnection _connection;

            public EventStoreSubscriptions(SqlStoreConnection connection)
            {
                _connection = connection;
            }

            public void SubscribeToStreams<T>(int eventNumber, Action<IEvent> action)
            {
                foreach (var streamName in _connection.GetStreams(typeof(T)).Select(x => x.Name))
                    SubscribeToStream(streamName, eventNumber, action);
            }

            public void SubscribeToStream(string streamName, int eventNumber, Action<IEvent> action)
            {
                var now = _connection._timeline.Now();
                var events = _connection.ReadStreamEventsForward(streamName, eventNumber, int.MaxValue)
                    .Where(e => e.Timestamp <= now)
                    .ToList().OrderBy(e => e.Timestamp);

                foreach (var e in events)
                    action(e);

                _eventAppended[action] = (o, e) =>
                {
                    if (e.StreamName == streamName && _connection._timeline.Now() >= e.Event.Timestamp)
                        action(e.Event);
                };

                EventAppended += _eventAppended[action];
            }

            public void DropSubscription(string streamName, Action<IEvent> action)
            {
                EventAppended -= _eventAppended[action];
            }

            public void OnEventAppended(string streamName,IEvent e)
            {
                EventAppended?.Invoke(null, new EventAppendedArgs(streamName, e));
            }

            public void OnStreamAdded(string streamName)
            {
                StreamAdded?.Invoke(null, new StreamAddedArgs(streamName));
            }

        }
    }
}