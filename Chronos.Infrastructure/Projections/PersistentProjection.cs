using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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

        private Action<IEnumerable<T>, IEnumerable<IEvent>> _action = (x, e) => { };

        public override void Do<TS>(Action<IEnumerable<TS>,IEnumerable<IEvent>> action)
        {
            _action = (x,e) => action(x as IEnumerable<TS>, e);
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

        protected virtual void Write(TKey key, IList<IEvent> events)
        {
            _writer.Write<TKey,T>(key,x =>
            {
                x.Timeline = Timeline;
                var changed = false;
                var models = new List<T>();
                foreach (var e in events)
                {
                    changed |= x.When(e);
                    models.Add(x);
                }
                _action(models, events);

                return changed;
            });    
        }
        
        protected override void When(StreamDetails stream, IList<IEvent> events)
        {
            foreach (var key in Key.Get(stream))
                Write(key, events);

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