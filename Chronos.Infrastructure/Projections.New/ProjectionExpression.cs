using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Chronos.Infrastructure.Events;
using NodaTime;

namespace Chronos.Infrastructure.Projections.New
{
    public class ProjectionExpression<T> : IBaseProjectionExpression<T>, ITransientProjectionExpression<T>, IPersistentProjectionExpression<T>
        where T : class, IReadModel,new()
    {
        private readonly IEventStoreSubscriptions _eventStore;
        private readonly IStateWriter _writer;
        private readonly IEventBus _eventBus;

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

        public void Compile() => Start();
        
        IPersistentProjection<T> IPersistentProjectionExpression<T>.Compile()
        {
            Start();
            return Projection as IPersistentProjection<T>;
        }

        ITransientProjection<T> ITransientProjectionExpression<T>.Compile()
        {
            Start();
            return Projection as ITransientProjection<T>;
        }
        
        private IObservable<StreamDetails> _streams;
        private bool _forEachStream;
        
        private readonly ReplaySubject<Projection> _projectionSubject = new ReplaySubject<Projection>();

        public IProjectionExpression<T> Clone()
        {
            return new ProjectionExpression<T>(this);
        }

        private void Start() => _projectionSubject.Subscribe(p => p.Start());
        
        private ProjectionExpression(ProjectionExpression<T> rhs)
        {
            _streams = rhs._streams;
            _forEachStream = rhs._forEachStream;

            _eventBus = rhs._eventBus;
            _eventStore = rhs._eventStore;
            _writer = rhs._writer;
            
            _projectionSubject.Subscribe(p => _eventBus.Subscribe<ReplayCompleted>(e => p.OnReplay()));  
        }
        
        public ProjectionExpression(IEventStoreSubscriptions eventStore, IStateWriter writer, IEventBus eventBus)
        {
            _eventStore = eventStore;
            _writer = writer;
            _eventBus = eventBus;

            _streams = _eventStore.Streams;
            
            _projectionSubject.Subscribe(p => _eventBus.Subscribe<ReplayCompleted>(e => p.OnReplay()));  
        }

        public IProjectionExpression<T> From<TAggregate>()
            where TAggregate : IAggregate
        {
            _streams = _streams.Where(s => s.SourceType == typeof(TAggregate).Name);
            return this;
        }

        public IProjectionExpression<T> From<TAggregate>(Guid id) where TAggregate : IAggregate
        {
            From<TAggregate>();
            _streams = _streams.Where(s => s.Key == id);
            return this;
        }

        public ITransientProjectionExpression<T> Transient()
        {
            Projection = new TransientProjection<T>(_eventStore)
            {
                Streams = _streams
            };

            return this;
        }

        public ITransientProjectionExpression<T> AsOf(Instant date)
        {
            Projection = new HistoricalProjection<T>(_eventStore, date)
            {
                Streams = _streams
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
            
            Projection = new PersistentPartitionedProjection<T>(_eventStore,_writer);

            return this;
        }

        public IPersistentProjectionExpression<T> OutputState<TKey>(TKey key) where TKey : IEquatable<TKey>
        {
            if(_forEachStream)
                throw new InvalidOperationException("Cannot output partitioned projection to single state");
            
            Projection = new PersistentProjection<TKey,T>(_eventStore,_writer)
            {
                Key = s => key
            };

            return this;
        }


    }
}