using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;

namespace Chronos.Infrastructure.Projections
{
    public class SelectProjection<TKey,TSource,TResult> : Projection
        where TKey : IEquatable<TKey> 
        where TSource : class, IReadModel, new()
        where TResult : class, IReadModel, new()
    {
        private readonly Projection _projection;
        private readonly IStateWriter _stateWriter;
        private readonly Func<TSource, Action<TResult>> _selector;

        public SelectProjection(Projection projection, Func<TSource,Action<TResult>> selector, 
            IEventStore eventStore, IStateWriter stateWriter)
            : base(eventStore)
        {
            _selector = selector;
            _projection = projection;
            _stateWriter = stateWriter;
        }

        private void Write(TKey key, IList<Action<TResult>> actions)
        {
            _stateWriter.Write<TKey, TResult>(key, x =>
            {
                foreach (var action in actions)
                    action(x);
                return true;
            });
        }

        public override void Start(bool reset = false)
        {
            if (!(_projection is PersistentProjection<TKey, TSource> projection))
                return;
                //throw new InvalidOperationException();

            projection.Models.GroupBy(x => ((IReadModel<TKey>) x).Key)
                .Subscribe(x => x.Select(source => _selector(source))
                    .Window(() => projection.Windows)
                    //.Buffer(TimeSpan.FromMilliseconds(50), 1000)
                    .Subscribe(w => w.Buffer(TimeSpan.FromMilliseconds(25),1000)
                        .Where(buffer => buffer.Any())
                    .Subscribe(list => Write(x.Key,list))));
            //.Where(buffer => buffer.Any())
            //.ObserveOn(Scheduler.Default)
            //.Subscribe(actions => Write(x.Key, actions)));

            /*projection.Models.Subscribe(o =>
            {
                var allModels = o.GroupBy(x => ((IReadModel<TKey>) x).Key);

                allModels.Subscribe(x => _stateWriter.Write<TKey, TResult>(x.Key, z =>
                {
                    foreach (var enumerable in x.Chunkify())
                    foreach (var y in enumerable)
                        _selector(y)(z);
                    return true;
                }));
                /*allModels.Subscribe(x => x.Subscribe(y =>
                    _stateWriter.Write<TKey, TResult>(x.Key, z =>
                    {
                        _selector(y)(z);
                        return true;
                    })));*/
            //});

            /*projection.Models.Subscribe(o =>
            {
                var allModels = o.GroupBy(x => ((IReadModel<TKey>) x).Key);

                allModels.Subscribe(x => _stateWriter.Write<TKey, TResult>(x.Key, z =>
                {
                    foreach (var enumerable in x.Chunkify())
                        foreach (var y in enumerable)
                            _selector(y)(z);
                    return true;
                }));
                /*allModels.Subscribe(x => x.Subscribe(y =>
                    _stateWriter.Write<TKey, TResult>(x.Key, z =>
                    {
                        _selector(y)(z);
                        return true;
                    })));*/
            //});
        }
    }
}