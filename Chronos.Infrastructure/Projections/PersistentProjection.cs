using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Xml.Linq;
using Chronos.Infrastructure.Interfaces;

namespace Chronos.Infrastructure.Projections
{
    public class PersistentProjection<TKey,T> : Projection, IPersistentProjection<T>
        where T : class, IReadModel, new()
        where TKey : IEquatable<TKey>
    {
        protected class KeySelector
        {
            private readonly Func<StreamDetails, TKey> _keyFunc;
            private readonly ConcurrentDictionary<TKey, byte> _keys = new ConcurrentDictionary<TKey, byte>();
            private readonly Func<string, bool> _typePredicate;
            
            public KeySelector(Func<StreamDetails, TKey> keyFunc, Func<string, bool> typePredicate = null)
            {
                _keyFunc = keyFunc;
                _typePredicate = typePredicate ?? (x => true);
            }

            public IEnumerable<TKey> Get(StreamDetails stream)
            {
                // return all possible keys for events from auxilary streams
                if (!_typePredicate(stream.SourceType))
                    return new List<TKey>(_keys.Keys);
               
                var key = _keyFunc(stream);
                return new List<TKey> { key };
            }

            public void Add(StreamDetails stream)
            {
                _keys.TryAdd(_keyFunc(stream), 0);
            }

            public bool Has(StreamDetails stream)
            {
                return _typePredicate(stream.SourceType);
            }
        }
        
        protected KeySelector Key { private get; set; }
        private readonly IStateWriter _writer;
        private readonly IReadRepository _readRepository;

        //private readonly Subject<IConnectableObservable<T>> _models = new Subject<IConnectableObservable<T>>();
        private readonly Subject<T> _models = new Subject<T>();

        //public IObservable<IConnectableObservable<T>> Models => _models.AsObservable();
        public IObservable<T> Models => _models.AsObservable();
        
        private readonly Subject<bool> _openingWindow = new Subject<bool>();
        public IObservable<bool> OpeningWindow => _openingWindow;
        
        private readonly Subject<bool> _closingWindow = new Subject<bool>();
        public IObservable<bool> ClosingWindow => _closingWindow.AsObservable();

        private Action<T> _action;
        
        public override void Do<TS>(Action<TS> action)
        {
            _action = x => action(x as TS); 
            base.Do(action);
        }

        internal PersistentProjection(IEventStore eventStore,IStateWriter writer, IReadRepository readRepository)
            : base(eventStore)
        {
            _writer = writer;
            _readRepository = readRepository;
        }

        protected override void Register(IObservable<StreamDetails> streams)
        {
            streams.Subscribe(x => Key.Add(x));
            base.Register(streams);
        }

        public override void Start(bool reset = false)
        {
            if (typeof(T).GetTypeInfo().GetCustomAttributes<ResetAttribute>().Any())
                reset = true;
            
            base.Start(reset);
        }

        protected T Get(TKey key)
        {
            return _readRepository.Find<TKey,T>(key); 
        }

        protected virtual void Write(IEnumerable<TKey> keys, IList<IEvent> events)
        {
            if (events.Count == 0)
                return;
            
            _writer.Write<TKey,T>(keys,x =>
            {
                x.Timeline = Timeline;
                var changed = false;

                _openingWindow.OnNext(true);
                foreach (var e in events)
                {
                    changed |= x.When(e);
                    _action(x);
                    _models.OnNext(x);
                }
                _closingWindow.OnNext(true);
                return changed;
            });     
        }
        
        protected virtual void Write(TKey key, IList<IEvent> events)
        {
            if (events.Count == 0)
                return;
            
            _writer.Write<TKey,T>(key,x =>
            {
                x.Timeline = Timeline;
                var changed = false;

                _openingWindow.OnNext(true);
                foreach (var e in events)
                {
                    changed |= x.When(e);
                    _action(x);
                    _models.OnNext(x);
                }
                _closingWindow.OnNext(true);
                /*var observable = events.ToObservable().Select(e =>
                {
                    changed |= x.When(e);
                    return x;
                }).Publish(); 
                
                _models.OnNext(observable);
                observable.Connect();*/
                //observable.Subscribe(y => y.ToString());
                //observable.Wait();
                //_models.OnNext(observable);
                //observable.Connect();
                //observable.Wait();
                
                return changed;
            });    
        }
        
        protected override void When(StreamDetails stream, IList<IEvent> events)
        {
            //foreach (var key in Key.Get(stream))
            //    Write(key, events);
            var keys = Key.Get(stream);
            Write(keys,events);

            base.When(stream, events);
        }

        protected override int GetVersion(StreamDetails stream)
        {
            if (!Key.Has(stream))
                return -1;

            var readModel = Get(Key.Get(stream).SingleOrDefault());
            if (readModel == null)
                return -1;
            return readModel.Version;
        }
    }
}