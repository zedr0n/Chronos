using System;

namespace Chronos.Infrastructure
{
    public interface IReadRepository
    {
        T Find<TKey, T>(TKey key) where T : class,IReadModel
            where TKey : IEquatable<TKey>;

        T Find<T>(Func<T, bool> predicate) where T : class, IReadModel;
    }
    
    public interface IMemoryReadRepository : IReadRepository {
        void Set<TKey, T>(TKey key, T readModel) where T : class, IReadModel
            where TKey : IEquatable<TKey>;

        T GetOrAdd<TKey, T>(TKey key)
            where T : class, IReadModel, new()
            where TKey : IEquatable<TKey>;
    }
}