using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
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
            private readonly ReplaySubject<StreamDetails> _streams;
            private readonly Dictionary<string,Subject<IEvent>> _events;
            public IObservable<StreamDetails> Streams { get; }

            private readonly SqlStoreConnection _connection;

            internal EventStoreSubscriptions(SqlStoreConnection connection)
            {
                _connection = connection;
                _streams = new ReplaySubject<StreamDetails>();
                _events = new Dictionary<string, Subject<IEvent>>();

                Streams = _connection.GetStreams().ToObservable().Concat(_streams);
            }

            public IObservable<IEvent> GetEvents(StreamDetails stream, int eventNumber)
            {
                var events = _connection.ReadStreamEventsForward(stream.Name, eventNumber, int.MaxValue)
                    .ToList()
                    .OrderBy(e => e.Timestamp);

                if (!_events.ContainsKey(stream.Name))
                    _events[stream.Name] = new Subject<IEvent>();

                return _events[stream.Name].StartWith(events).Where(e => e.Timestamp <= _connection._timeline.Now());
            }

            internal void OnEventAppended(StreamDetails stream,IEvent e)
            {
                if(_events.ContainsKey(stream.Name))
                    _events[stream.Name].OnNext(e);
            }

            internal void OnStreamAdded(StreamDetails details)
            {
                _streams.OnNext(details);
            }

        }
    }
}