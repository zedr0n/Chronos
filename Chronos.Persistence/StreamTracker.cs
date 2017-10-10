using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using Chronos.Infrastructure;

namespace Chronos.Persistence
{
    public class StreamTracker
    {
        private readonly List<StreamDetails> _streams = new List<StreamDetails>();
        private readonly IDisposable _subscription;
        private readonly ITimeline _timeline;
        private readonly IAggregateFactory _aggregateTracker;

        public StreamTracker(ITimeline timeline, IEventStoreConnection connection, IAggregateFactory aggregateTracker)
        {
            _timeline = timeline;
            _aggregateTracker = aggregateTracker;
            _subscription = connection.GetStreams().ToObservable()
                .Concat(connection.Events.Select(e => e.Stream))
                .Subscribe(Set); 
        }

        private void Set(StreamDetails stream)
        {
            lock (_streams)
            {
                var existing = _streams
                    .SingleOrDefault(s => s.Name == stream.Name && s.Timeline == stream.Timeline);

                if (existing != null)
                    _streams.Remove(existing);
            
                _streams.Add(stream); 
            }

        }
        
        public StreamDetails Add(IAggregate aggregate)
        {
            lock (_streams)
            {
                if(_streams.Any(s => s.Key == aggregate.Id
                                     && s.Timeline == _timeline.TimelineId
                                     && s.SourceType == aggregate.GetType().SerializableName()))
                    throw new InvalidOperationException("Aggregate stream already exists");
                var stream = new StreamDetails(aggregate)
                {
                    Timeline = _timeline.TimelineId
                };
                _streams.Add(stream);
                return stream;
            }
        }

        public StreamDetails Get(StreamDetails request)
        {
            lock (_streams)
            {
                return _streams.Single(s => s.Name == request.Name && s.Timeline == _timeline.TimelineId);
            }
        }
        
        public StreamDetails Get(IAggregate aggregate)
        {
            lock (_streams)
            {
                return _streams.Single(s => s.Key == aggregate.Id && s.Timeline == _timeline.TimelineId
                                            && s.SourceType == aggregate.GetType().SerializableName());
            }
        }

        public StreamDetails Add(ISaga saga)
        {
            lock (_streams)
            {
                if(_streams.Any(s => s.Key == saga.SagaId && s.Timeline == _timeline.TimelineId && s.SourceType == saga.GetType().SerializableName()))
                    throw new InvalidOperationException("Saga stream already exists");
                var stream = new StreamDetails(saga)
                {
                    Timeline = _timeline.TimelineId
                };
                _streams.Add(stream);
                return stream;
            }
        }

        public StreamDetails Get(ISaga saga)
        {
            lock (_streams)
            {
                return _streams.Single(s => s.Key == saga.SagaId && s.Timeline == _timeline.TimelineId && 
                                            s.SourceType == saga.GetType().SerializableName());
            }
        }

        public StreamDetails Add(Type aggregateType, Guid id)
        {
            lock (_streams)
            {
                if (_streams.Any(s =>
                    s.Key == id && s.SourceType == aggregateType.SerializableName() && s.Timeline == _timeline.TimelineId))
                    throw new InvalidOperationException("Stream already exists");
                var stream = new StreamDetails(aggregateType, id)
                {
                    Timeline = _timeline.TimelineId
                };
                return stream;
            }
        }
        
        public StreamDetails GetSaga(Type sagaType, Guid id)
        {
            lock (_streams)
            {
                return _streams.SingleOrDefault(s => s.IsSaga &&
                    s.Key == id && s.SourceType == sagaType.SerializableName() && s.Timeline == _timeline.TimelineId);
            }
        }

        public StreamDetails Get<T>(Guid id)
            where T : IAggregate
        {
            lock (_streams)
            {
                return _streams.SingleOrDefault(s => !s.IsSaga &&
                    s.Key == id && _aggregateTracker.Is<T>(s.SourceType) && s.Timeline == _timeline.TimelineId);
            }
        }
        
    }
}