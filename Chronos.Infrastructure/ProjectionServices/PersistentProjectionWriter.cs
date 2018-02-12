using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Chronos.Infrastructure.Interfaces;
using Chronos.Infrastructure.Projections;

namespace Chronos.Infrastructure.ProjectionServices
{
    public class PersistentProjectionWriter<TKey,T> : IProjectionWriter<TKey,T> 
        where T : class, IReadModel, new()
        where TKey : IEquatable<TKey>
    {
        private readonly IStateWriter _writer;
        private readonly ITimeline _timeline;
        
        public void Write(IEnumerable<TKey> keys, IList<IEvent> events)
        {
            if (events.Count == 0)
                return;

            var timeline = _timeline.TimelineId;
            _writer.Write<TKey,T>(keys,x =>
            {
                x.Timeline = timeline;
                var changed = false;

                _openingWindow.OnNext(true);
                
                foreach (var e in events)
                    changed |= Write(x, e);

                _closingWindow.OnNext(true);
                
                return changed;
            });    
        }

        protected virtual bool Write(T model, IEvent e)
        {
            var changed = model.When(e);
            _models.OnNext(model);
            return changed;
        }

        private readonly Subject<T> _models = new Subject<T>();
        private readonly Subject<bool> _openingWindow = new Subject<bool>();
        private readonly Subject<bool> _closingWindow = new Subject<bool>();

        public PersistentProjectionWriter(IStateWriter writer, ITimeline timeline)
        {
            _writer = writer;
            _timeline = timeline;
        }

        public IObservable<bool> OpeningWindow => _openingWindow.AsObservable();
        public IObservable<bool> ClosingWindow => _closingWindow.AsObservable();
        public IObservable<T> Models => _models.AsObservable();
    }
    
    public class PersistentStreamProjectionWriter<T> : PersistentProjectionWriter<Guid,T>, IStreamProjectionWriter<T>
        where T : class, IReadModel, new()
    {
        public PersistentStreamProjectionWriter(IStateWriter writer, ITimeline timeline) : base(writer, timeline)
        {
        }
    }
}
