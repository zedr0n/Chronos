using System;
using System.Linq;
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