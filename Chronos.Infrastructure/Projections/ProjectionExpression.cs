using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Runtime.CompilerServices;
using Chronos.Infrastructure.Events;
using Chronos.Infrastructure.Interfaces;
using NodaTime;

namespace Chronos.Infrastructure.Projections
{
    public class ProjectionExpression<T> : IBaseProjectionExpression<T>, ITransientProjectionExpression<T>, IPersistentProjectionExpression<T>
        where T : class, IReadModel,new()
    {
        private readonly IEventStore _eventStore;
        private readonly IStateWriter _writer;
        private readonly IReadRepository _readRepository;

        private string _keyAggregateType;
        private Action<T> _action = (x) => { };

        private Projection _projection;
        private Projection Projection
        {
            get => _projection;
            set
            {
                _projection = value;   
                _projectionSubject.OnNext(_projection);
            }        
        }

        public void Invoke() => Start();
        
        IPersistentProjection<T> IPersistentProjectionExpression<T>.Invoke()
        {
            Start();
            return Projection as IPersistentProjection<T>;
        }

        ITransientProjection<T> ITransientProjectionExpression<T>.Invoke()
        {
            Start();
            return Projection as ITransientProjection<T>;
        }

        private Selector<StreamDetails> _streamSelector = new Selector<StreamDetails>();
        private bool _forEachStream;
        
        private readonly ReplaySubject<Projection> _projectionSubject = new ReplaySubject<Projection>();

        public IProjectionExpression<T> Clone()
        {
            return new ProjectionExpression<T>(this);
        }

        private void Start() => _projectionSubject.Subscribe(p => p.Start());
        
        private ProjectionExpression(ProjectionExpression<T> rhs)
        {
            _readRepository = rhs._readRepository;
            _forEachStream = rhs._forEachStream;

            _eventStore = rhs._eventStore;
            _writer = rhs._writer;

            _projectionSubject.Subscribe(p =>
                //_eventStore.AggregateEvents.OfType<ReplayCompleted>().Subscribe(e => p.OnReplay()));
                //_eventStore.ReplayCompleted.Subscribe(e => p.OnReplay()));
                    _eventStore.Alerts.OfType<ReplayCompleted>().Subscribe(e => p.Start(true)));

            //_projectionSubject.Subscribe(p => _eventBus.Subscribe<ReplayCompleted>(e => p.OnReplay()));  
        }
        
        public ProjectionExpression(IEventStore eventStore, IStateWriter writer, IReadRepository readRepository)
        {
            _eventStore = eventStore;
            _writer = writer;
            _readRepository = readRepository;

            _streamSelector = new Selector<StreamDetails>().Where(s => !s.IsSaga);

            _projectionSubject.Subscribe(p =>
                //_eventStore.AggregateEvents.OfType<ReplayCompleted>().Subscribe(e => p.OnReplay()));
                    //_eventStore.ReplayCompleted.Subscribe(e => p.OnReplay()));
                    _eventStore.Alerts.OfType<ReplayCompleted>().Subscribe(e => p.Start(true)));

            //_projectionSubject.Subscribe(p => _eventBus.Subscribe<ReplayCompleted>(e => p.OnReplay()));  
        }

        public IProjectionExpression<T> From<TAggregate>()
            where TAggregate : IAggregate
        {
            _keyAggregateType = typeof(TAggregate).SerializableName();
            _streamSelector = _streamSelector.Where(x => x.SourceType == _keyAggregateType);
            
            return this;
        }

        public IProjectionExpression<T> From<TAggregate>(Guid id) where TAggregate : IAggregate
        {
            From<TAggregate>();
            _streamSelector = _streamSelector.Where(x => x.Key == id);
            return this;
        }

        public IProjectionExpression<T> Include<TAggregate>() where TAggregate : IAggregate
        {
            _streamSelector = _streamSelector.Or(x => x.SourceType == typeof(TAggregate).SerializableName());
            return this;
        }

        public IProjectionExpression<TResult> Select<TKey,TResult>(Func<T, Action<TResult>> selector)
            where TResult : class, IReadModel, new() 
            where TKey : IEquatable<TKey>
        {
            var projection = new SelectProjection<TKey,T,TResult>(Projection,selector,
                _eventStore,_writer);
            return new ProjectionExpression<TResult>(_eventStore, _writer, _readRepository)
            {
                Projection = projection
            };
        }

        public IProjectionExpression<T> Do(Action<T> action)
        {
            _action = action;
            return this;
        }

        public ITransientProjectionExpression<T> Transient()
        {
            Projection = new TransientProjection<T>(_eventStore)
            {
                Selector = _streamSelector
            };

            return this;
        }

        public ITransientProjectionExpression<T> AsOf(Instant date)
        {
            Projection = new HistoricalProjection<T>(_eventStore,date)
            {
                Selector = _streamSelector
            };

            return this;
        }

        public IProjectionExpression<T> ForEachStream()
        {
            _forEachStream = true;
            return this;
        }

        public IPersistentProjectionExpression<T> OutputState()
        {
            if(!_forEachStream)
                throw new InvalidOperationException("No key defined for projection state persistence");
           
            Projection = new PersistentKeyedProjection<T>(_eventStore, _writer, _readRepository)
            {
                Selector = _streamSelector,
                KeyAggregateType = _keyAggregateType
            };
            
            Projection.Do(_action);

            return this;
        }

        public IPersistentProjectionExpression<T> OutputState<TKey>(TKey key) where TKey : IEquatable<TKey>
        {
            if(_forEachStream)
                throw new InvalidOperationException("Cannot output partitioned projection to single state");
            
            Projection = new MultiProjection<TKey,T>(_eventStore,_writer,_readRepository)
            {
                Selector = _streamSelector,
                Key = key
            };
            
            Projection.Do(_action);

            return this;
        }


    }
}