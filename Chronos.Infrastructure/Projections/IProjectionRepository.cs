using System;
using System.Collections.Generic;
using NodaTime;

namespace Chronos.Infrastructure.Projections
{
    public interface IProjectionRepository
    {
        /// <summary>
        /// Find all projections satisfying the criteria or return null
        /// </summary>
        /// <param name="criteria">Criteria projections need to satisfy</param>
        /// <returns>All projections satisfying the criteria or null if none found</returns>
        IEnumerable<T> Find<T>(Func<T, bool>  criteria) where T : class,IProjection;

        T Find<TKey, T>(TKey key) where T : class, IProjection<TKey>
                                  where TKey : IEquatable<TKey>;

        T Find<TKey, T>(HistoricalKey<TKey> key) where TKey : IEquatable<TKey>
            where T : class, IProjection<TKey>, new();

        IEnumerable<T> Get<T>(Func<T,bool> criteria) where T : class, IProjection;
        /// <summary>
        /// Add new projection to repository
        /// </summary>
        /// <param name="projection">Projection instance</param>
        /// <returns>All projections satisfying the criteria or null if none found</returns>
        void Add<T>(T projection) where T : IProjection;
    }
}