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
    public class Envelope
    {       
        public IEvent Event { get; }
        public StreamDetails Stream { get; }
        
        public Envelope(IEvent @event, StreamDetails stream)
        {
            Event = @event;
            Stream = stream;
        }
    }
    
    public partial class SqlStoreConnection
    {
        private class EventStoreSubscriptions : IEventStoreSubscriptions
        {
            private readonly ReplaySubject<StreamDetails> _streams;
            //private readonly Subject<IEvent> _allEvents = new Subject<IEvent>();
            //private readonly Dictionary<string,Subject<IEvent>> _events;
            private readonly Subject<Envelope> _events = new Subject<Envelope>();
            public IObservable<StreamDetails> Streams { get; }

            private readonly SqlStoreConnection _connection;

            internal EventStoreSubscriptions(SqlStoreConnection connection)
            {
                _connection = connection;
                _streams = new ReplaySubject<StreamDetails>();
                //_events = new Dictionary<string, Subject<IEvent>>();

                Streams = _connection.GetStreams().ToObservable().Concat(_streams.AsObservable());
            }

            public IObservable<IEvent> Events => _events.AsObservable().Select(env => env.Event);

            public IObservable<IEvent> AggregateEvents => _events.AsObservable()
                .Where(env => !env.Stream.Name.Contains("Saga"))
                .Select(env => env.Event);

            public IObservable<StreamDetails> GetStreams()
            {
                return Observable.Create((IObserver<StreamDetails> observer) =>
                {
                    var streams = _connection.GetStreams();
                    foreach (var s in streams)
                        observer.OnNext(s);

                    var subscription = _streams.Subscribe(observer.OnNext);

                    return Disposable.Create(() => subscription.Dispose());
                });
            }

            public IObservable<IEvent> GetEventsEx(StreamDetails stream, int eventNumber)
            {
                return Observable.Create((IObserver<IEvent> observer) =>
                {
                    var events = _connection.ReadStreamEventsForward(stream.Name, eventNumber, int.MaxValue)
                        .OrderBy(e => e.Timestamp)
                        .Where(e => e.Timestamp <= _connection._timeline.Now());
                    foreach (var e in events)
                        observer.OnNext(e);

                    var subscription = _events.Where(e => e.Stream.Name == stream.Name)
                        .Where(e => e.Event.Timestamp <= _connection._timeline.Now())
                        .Subscribe(env => observer.OnNext(env.Event)); 

                    return Disposable.Create(() => subscription.Dispose() );
                });
            }
            
            public IObservable<IEvent> GetEvents(StreamDetails stream, int eventNumber)
            {
                var events = _connection.ReadStreamEventsForward(stream.Name, eventNumber, int.MaxValue)
                    //.ToList()§
                    .OrderBy(e => e.Timestamp);

                //if (!_events.ContainsKey(stream.Name))
                //    _events[stream.Name] = new Subject<IEvent>();

                return null;
                //return _events[stream.Name].StartWith(events).Where(e => e.Timestamp <= _connection._timeline.Now());
            }

            public IObservable<TEvent> GetEvents<TEvent>() where TEvent : IEvent
            {
                return Events.OfType<TEvent>();
            }

            internal void OnEventAppended(StreamDetails stream,IEvent e)
            {
                _events.OnNext(new Envelope(e, stream));
            }

            internal void OnStreamAdded(StreamDetails details)
            {
                _streams.OnNext(details);
            }

        }
    }
}