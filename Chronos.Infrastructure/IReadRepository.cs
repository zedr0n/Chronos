using System;

namespace Chronos.Infrastructure
{
    public interface IReadRepository
    {
        T Find<TKey, T>(TKey key) where T : class,IReadModel
            where TKey : IEquatable<TKey>;

        void Add<T>(T readModel) where T : IReadModel;

    }
}