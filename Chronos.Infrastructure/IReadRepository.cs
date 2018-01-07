using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Chronos.Infrastructure
{
    public interface IReadRepository
    {
        T Find<TKey, T>(TKey key) where T : class,IReadModel
            where TKey : IEquatable<TKey>;

        T Find<TKey, T, TProperty>(TKey key, Expression<Func<T, IEnumerable<TProperty>>> include) where T : class,IReadModel
            where TKey: IEquatable<TKey>
            where TProperty : class;

        T Find<T>(Func<T, bool> predicate) where T : class, IReadModel;
        T GetOrAdd<TKey, T>(TKey key)
            where T : class, IReadModel, new()
            where TKey : IEquatable<TKey>;
    }
    
    public interface IMemoryReadRepository : IReadRepository {
        
        void Set<TKey, T>(TKey key, T readModel) where T : class, IReadModel
            where TKey : IEquatable<TKey>;
    }
}