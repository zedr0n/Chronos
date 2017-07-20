using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Chronos.Infrastructure.Interfaces;
using NodaTime;

namespace Chronos.Infrastructure.Projections.New
{
    public abstract class ProjectionBase<TKey,T> : IProjection<TKey,T> where T : class, IReadModel<TKey>, new()
                                                                       where TKey : IEquatable<TKey>
    {
        private readonly IStateWriter _writer;
        private readonly IEventStoreConnection _connection;
        private IEnumerable<string> _streams;

        // state update action
        private Action<IEvent> _action;

        // in-memory projection state
        public T State { get; } = new T();

        private readonly Dictionary<Type,Action<T, IEvent>> _when = new Dictionary<Type, Action<T, IEvent>>();

        // stream name => output key
        private readonly Dictionary<string, TKey> _output = new Dictionary<string, TKey>();

        private void RegisterHandlers()
        {
            _when.Clear();
            foreach (var m in GetType().GetRuntimeMethods())
            {
                var parameters = m.GetParameters().ToList();
                if (parameters.Count != 2)
                    continue;

                if(parameters[1].ParameterType != typeof(IEvent))
                    _when.Add(parameters[1].ParameterType, (s,e) => m.Invoke(this,new object[] {s,e}));
            }
        }

        protected ProjectionBase<TKey,T> Clone()
        {
           var projection = (ProjectionBase<TKey,T>) MemberwiseClone();
           projection.RegisterHandlers();
           return projection;
        }

        protected ProjectionBase(IStateWriter stateWriter, IEventStoreConnection connection)
        {
            _writer = stateWriter;
            _connection = connection;
            RegisterHandlers();
        }

        public IProjection<TKey,T> From(IEnumerable<string> streams)
        {
            var projection = Clone();
            projection._streams = streams;
            projection._action = e => When(projection.State,e);

            return projection;
        }

        public IProjection<TKey, T> ForEachStream(Func<string,TKey> map)
        {
            _output.Clear();
            foreach (var s in _streams)
                _output.Add(s, map(s));

            return this;
        }

        public IProjection<TKey, T> ForAllStreams(TKey key)
        {
            _output.Clear();
            foreach (var s in _streams)
                _output.Add(s, key);

            return this;
        }

        public IProjection<TKey, T> OutputState()
        {
            _action = e =>
            {
                foreach (var key in _streams.Select(s => _output[s]).Distinct())
                    _writer.Write<TKey,T>(key, x => When(x,e));
            };
            return this;
        }

        public IProjection<TKey, T> AsOf(Instant date)
        {
            var projection = new HistoricalProjection<TKey, T>(_writer,_connection)
            {
                Date = date,
                _streams = _streams,
                Projection = this
            };
            return projection;
        }


        public void Start()
        {
            foreach(var s in _streams)
                _connection.SubscribeToStream(s,-1,When);
        }

        public virtual void When(T state, IEvent e)
        {
            if (_when.ContainsKey(e.GetType()))
                _when[e.GetType()](state, e);
        }

        public virtual void When(IEvent e)
        {
            _action(e);
        }

    }
}