using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;

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

        IEnumerable<T> Get<T>(Func<T,bool> criteria) where T : class, IProjection;
        /// <summary>
        /// Add new projection to repository
        /// </summary>
        /// <param name="projection">Projection instance</param>
        /// <returns>All projections satisfying the criteria or null if none found</returns>
        void Add<T>(T projection) where T : IProjection;
    }
}