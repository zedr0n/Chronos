﻿using System;
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

        public StreamTracker(ITimeline timeline, IEventStoreConnection connection)
        {
            _timeline = timeline;
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
                if(_streams.Any(s => s.Key == aggregate.Id && s.Timeline == _timeline.TimelineId))
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
                return _streams.Single(s => s.Key == aggregate.Id && s.Timeline == _timeline.TimelineId);
            }
        }

        public StreamDetails Add(ISaga saga)
        {
            lock (_streams)
            {
                if(_streams.Any(s => s.Key == saga.SagaId && s.Timeline == _timeline.TimelineId && s.SourceType == saga.GetType().Name))
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
                return _streams.Single(s => s.Key == saga.SagaId && s.Timeline == _timeline.TimelineId);
            }
        }

        public StreamDetails Add(Type aggregateType, Guid id)
        {
            lock (_streams)
            {
                if (_streams.Any(s =>
                    s.Key == id && s.SourceType == aggregateType.Name && s.Timeline == _timeline.TimelineId))
                    throw new InvalidOperationException("Stream already exists");
                var stream = new StreamDetails(aggregateType, id)
                {
                    Timeline = _timeline.TimelineId
                };
                return stream;
            }
        }

        public StreamDetails Get(Type sourceType, Guid id)
        {
            lock (_streams)
            {
                return _streams.SingleOrDefault(s =>
                    s.Key == id && s.SourceType == sourceType.Name && s.Timeline == _timeline.TimelineId);

            }
        }
        
    }
}