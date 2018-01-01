using System;

namespace Chronos.Infrastructure.Projections
{
    public interface IStateWriter
    {
        void Write<TKey, T>(TKey key, Func<T,bool> action)
            where TKey : IEquatable<TKey>
            where T : class, IReadModel, new();
    }
    
    public interface IMemoryStateWriter : IStateWriter {
        T GetState<TKey, T>(TKey key)
            where TKey : IEquatable<TKey>
            where T : class,IReadModel,new();

        void SetState<TKey, T>(TKey key, T state)
            where TKey : IEquatable<TKey>
            where T : class,IReadModel,new();
    }
}