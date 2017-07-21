using System;
using System.Collections.Generic;
using System.Linq;

namespace Chronos.Infrastructure
{
    public class ReadRepository : IReadRepository
    {
        private readonly Dictionary<Type, List<IReadModel>> _dictionary = new Dictionary<Type, List<IReadModel>>();

        public T Find<TKey, T>(TKey key) where T : class,IReadModel
            where TKey: IEquatable<TKey>
        {
            if (!_dictionary.TryGetValue(typeof(T), out var readModels) || !readModels.Any<IReadModel>())
                return null;

            var readModel = readModels.Cast<IReadModel<TKey>>()?.SingleOrDefault(p => p.Key.Equals(key));
            return readModel as T;

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