using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Chronos.Infrastructure;

namespace Chronos.Persistence
{
    public class SqlReadRepository : IReadRepository
    {
        private readonly IReadDb _db;

        public SqlReadRepository(IReadDb db)
        {
            _db = db;
        }

        public T Find<TKey, T>(TKey key) where TKey : IEquatable<TKey> where T : class, IReadModel
        {
            using (var context = _db.GetContext())
            {
                return context.Find<T>(key);
            }
        }

        public T Find<TKey, T,TProperty>(TKey key,Expression<Func<T,IEnumerable<TProperty>>> include) where TKey : IEquatable<TKey> where T : class, IReadModel
            where TProperty : class
        {
            using (var context = _db.GetContext())
            {
                var entity = context.Find<T>(key);
                if (entity == null)
                    return null;
                context.Entry(entity).Collection(include).Load();
                return entity;
            }
        }
        
        public T Find<T>(Func<T, bool> predicate) where T : class, IReadModel
        {
            using (var context = _db.GetContext())
            {
                return context.Set<T>().Where(predicate).SingleOrDefault();
            }
        }

        public T GetOrAdd<TKey, T>(TKey key) where TKey : IEquatable<TKey> where T : class, IReadModel, new()
        {
            var readModel = Find<TKey, T>(key);
            if (readModel != null)
                return readModel;

            using (var context = _db.GetContext())
            {
                readModel = new T();
                ((IReadModel<TKey>) readModel).Key = key;
                context.Add(readModel);
                return readModel;
            }
        }
    }
}