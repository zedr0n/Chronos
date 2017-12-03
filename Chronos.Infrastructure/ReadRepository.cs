using System;
using System.Collections.Generic;
using System.Linq;

namespace Chronos.Infrastructure
{
    public class ReadRepository : IMemoryReadRepository
    {
        private readonly Dictionary<Type, List<IReadModel>> _dictionary = new Dictionary<Type, List<IReadModel>>();

        private readonly ITimeline _timeline;

        public ReadRepository(ITimeline timeline)
        {
            _timeline = timeline;
        }

        public T Find<TKey, T>(TKey key) where T : class,IReadModel
            where TKey: IEquatable<TKey>
        {
            if (!_dictionary.TryGetValue(typeof(T), out var readModels) || !readModels.Any())
                return null;

            var readModel = readModels.Cast<IReadModel<TKey>>()?.SingleOrDefault(p => p.Key.Equals(key) && p.Timeline == _timeline.TimelineId);
            return readModel as T;
        }

        public T Find<T>(Func<T, bool> predicate) where T : class, IReadModel
        {
            if (!_dictionary.TryGetValue(typeof(T), out var readModels) || !readModels.Any())
                return null;

            return readModels.OfType<T>().Where(p => p.Timeline == _timeline.TimelineId).SingleOrDefault(predicate);
        }

        public void Add<T>(T readModel) where T : IReadModel
        {
            if(!_dictionary.ContainsKey(typeof(T)))
                _dictionary.Add(typeof(T),new List<IReadModel> { readModel });
            else
                _dictionary[typeof(T)].Add(readModel);
        }

    }
}