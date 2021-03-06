using System;
using System.Collections.Generic;

namespace Chronos.Infrastructure.Projections
{
    public class StateWriter : IMemoryStateWriter
    {
        private readonly IMemoryReadRepository _repository;

        public StateWriter(IMemoryReadRepository repository)
        {
            _repository = repository;
        }

        public T GetState<TKey, T>(TKey key)
            where TKey : IEquatable<TKey>
            where T : class,IReadModel,new()
        {
            return _repository.Find<TKey, T>(key);
        }
        
        public void SetState<TKey, T>(TKey key, T state)
            where TKey : IEquatable<TKey>
            where T : class,IReadModel,new()
        {
            _repository.Set(key,state);
        } 

        public virtual void Write<TKey, T>(TKey key, Func<T,bool> action)
            where TKey : IEquatable<TKey>
            where T : class,IReadModel,new()
        {
            var state = _repository.GetOrAdd<TKey, T>(key);
            action(state);
        }

        public void Write<TKey, T>(IEnumerable<TKey> keys, Func<T, bool> action) where TKey : IEquatable<TKey> where T : class, IReadModel, new()
        {
            foreach(var key in keys)
                Write(key, action);
        }
    }
}