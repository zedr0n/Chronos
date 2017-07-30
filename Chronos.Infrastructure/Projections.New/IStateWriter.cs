using System;

namespace Chronos.Infrastructure.Projections.New
{
    public interface IStateWriter
    {
        void Write<TKey, T>(TKey key, Action<T> action)
            where TKey : IEquatable<TKey>
            where T : class, IReadModel, new();

        void Write<T>(Func<T, bool> predicate, Action<T> action) where T : class, IReadModel, new();
    }
}