using System;

namespace Chronos.Infrastructure.Projections.New
{
    public class StateWriter : IStateWriter
    {
        private readonly IReadRepository _repository;

        public StateWriter(IReadRepository repository)
        {
            _repository = repository;
        }

        public virtual void Write<TKey, T>(TKey key, Action<T> action)
            where TKey : IEquatable<TKey>
            where T : class,IReadModel,new()
        {
            var state = _repository.Find<TKey, T>(key);
            if (state == null)
            {
                state = new T();
                ((IReadModel<TKey>)state).Key = key;
                _repository.Add(state);
            }

            action(state);
        }

        public void Write<T>(Func<T, bool> predicate, Action<T> action) where T : class, IReadModel, new()
        {
            var state = _repository.Find(predicate);
            if (state == null)
            {
                state = new T();
                action(state);
                _repository.Add(state);
            }
            else
                action(state);
        }
    }
}