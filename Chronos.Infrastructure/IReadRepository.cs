using System;

namespace Chronos.Infrastructure
{
    public interface IReadRepository
    {
        T Find<TKey, T>(TKey key) where T : class,IReadModel
            where TKey : IEquatable<TKey>;

        T Find<T>(Func<T, bool> predicate) where T : class, IReadModel;
        void Add<T>(T readModel) where T : IReadModel;
    }
}