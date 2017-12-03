using System;

namespace Chronos.Infrastructure.Projections.New
{
    public interface IStateWriter
    {
        void Write<TKey, T>(TKey key, Action<T> action)
            where TKey : IEquatable<TKey>
            where T : class, IReadModel, new();
    }
    
    public interface IMemoryStateWriter : IStateWriter {
        T GetState<TKey, T>(TKey key)
            where TKey : IEquatable<TKey>
            where T : class,IReadModel,new();
    }
}