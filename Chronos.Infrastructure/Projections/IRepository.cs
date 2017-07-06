using System;
using System.Collections.Generic;
using NodaTime;

namespace Chronos.Infrastructure.Projections
{
    public interface IRepository<T> where T : class
    {
        IEnumerable<T> All();
        T Get(Guid guid);
        void Add(Guid guid, T projection);

        /// <summary>
        /// Add a projection as of specified time, ignores duplicate projections
        /// </summary>
        /// <param name="guid">Projection id</param>
        /// <param name="projection"></param>
        /// <param name="asOf">Timestamp for projection</param>
        /// <remarks>Returns false if projection already exists in repository</remarks>
        /// <returns>All projections satisfying the criteria or null if none found</returns>
        bool Add(Guid guid, T projection, Instant asOf);
        /// <summary>
        /// Find all projections with the target source id satisfying the criteria or return null
        /// </summary>
        /// <param name="criteria">Criteria projections need to satisfy</param>
        /// <returns>All projections satisfying the criteria or null if none found</returns>
        IEnumerable<T> Find(Func<T, bool> criteria);
        /// <summary>
        /// Find all projections with the target source id satisfying the criteria or return null
        /// </summary>
        /// <param name="guid">Projection id</param>
        /// <param name="criteria">Criteria projections need to satisfy</param>
        /// <returns>All projections satisfying the criteria or null if none found</returns>
        IEnumerable<T> Find(Guid guid, Func<T, bool> criteria);
        /// <summary>
        /// Find all projections satisfying the time criteria or return null
        /// </summary>
        /// <param name="guid">Projection id</param>
        /// <param name="asOf">Before time instant</param>
        /// <returns>All projections satisfying the criteria or null if none found</returns>
        IEnumerable<T> Find(Guid guid, Instant asOf);
        /// <summary>
        /// Find all projections before asOf instant
        /// </summary>
        /// <exception cref="InvalidOperationException">No projections satisfy the criteria</exception>
        /// <param name="guid">Projection id</param>
        /// <param name="asOf">Before time instant</param>
        /// <returns>All projections satisfying the criteria or null if none found</returns>
        IEnumerable<T> Get(Guid guid, Instant asOf);
        /// <summary>
        /// Find all projections satisfying time criteria
        /// </summary>
        /// <exception cref="InvalidOperationException">No projections satisfy the criteria</exception>
        /// <param name="guid">Projection id</param>
        /// <param name="asOf">Before time instant</param>
        /// <returns>All projections satisfying the criteria or null if none found</returns>
        IEnumerable<T> Get(Guid guid, Func<T, bool> criteria);
        Guid ResolveId(T instance);
    }
}