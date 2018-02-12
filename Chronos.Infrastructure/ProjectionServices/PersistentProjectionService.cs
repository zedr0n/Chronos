using System;
using System.Collections.Generic;
using Chronos.Infrastructure.Interfaces;

namespace Chronos.Infrastructure.ProjectionServices
{
    public class PersistentProjectionService<TKey,T> : BaseProjection<T> where T : class, IReadModel, new() where TKey : IEquatable<TKey>
    {
        private readonly IKeySelector<TKey,T> _keySelector;
        private readonly IProjectionWriter<TKey,T> _writer;
        
        protected override void When(StreamDetails s, IList<IEvent> events)
        {
            var keys = _keySelector.Get(s);
            _writer.Write(keys,events);
            base.When(s, events);
        }

        public PersistentProjectionService(IEventStore eventStore, IStreamSelector<T> selector, IVersionProvider<T> versionProvider, IKeySelector<TKey, T> keySelector, IProjectionWriter<TKey, T> writer)
            : base(eventStore, selector, versionProvider)
        {
            _keySelector = keySelector;
            _writer = writer;
        }
    }
    
    public class StreamPersistentProjection<T> : PersistentProjectionService<Guid,T> where T : class, IReadModel, new()
    {
        public StreamPersistentProjection(IEventStore eventStore, IStreamSelector<T> selector, IVersionProvider<T> versionProvider, IKeySelector<Guid, T> keySelector, IProjectionWriter<Guid, T> writer)
            : base(eventStore, selector, versionProvider, keySelector, writer)
        {
        }
    }
}