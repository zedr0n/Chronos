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
            private readonly Subject<StreamDetails> _streams = new Subject<StreamDetails>();
            private readonly Subject<Envelope> _events = new Subject<Envelope>();
            
            private readonly SqlStoreConnection _connection;

            private readonly Dictionary<string, int> _versions = new Dictionary<string, int>();
            
            internal EventStoreSubscriptions(SqlStoreConnection connection)
            {
                _connection = connection;
                GetStreams().Subscribe(s => _versions[s.Name] = s.Version);
            }

            public IObservable<IEvent> Events => _events.AsObservable().Select(env => env.Event);

            public IObservable<IEvent> AggregateEvents => _events.AsObservable()
                .Where(env => !env.Stream.Name.Contains("Saga"))
                .Select(env => env.Event);

            public int GetStreamVersion(string streamName)
            {
                if (!_versions.ContainsKey(streamName))
                    return -1;
                return _versions[streamName];
            }
            
            public IObservable<StreamDetails> GetStreams()
            {
                return Observable.Create((IObserver<StreamDetails> observer) =>
                {
                    var streams = _connection.GetStreams();
                    foreach (var s in streams)
                        observer.OnNext(s);

                    var subscription = _streams.Subscribe(observer.OnNext);
                    return Disposable.Create(() => subscription.Dispose());
                })
                    .Where(s => !s.Name.Contains("Saga"));
            }

            public IObservable<IEvent> GetEvents(StreamDetails stream, int eventNumber)
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

            internal void OnEventAppended(StreamDetails stream,IEvent e)
            {
                _events.OnNext(new Envelope(e, stream));
                _versions[stream.Name] = stream.Version;
            }

            internal void OnStreamAdded(StreamDetails details)
            {
                _streams.OnNext(details);
            }

        }
    }
}