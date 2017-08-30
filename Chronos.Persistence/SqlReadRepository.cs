using System;
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
            throw new NotImplementedException();
        }

        public void Add<T>(T readModel) where T : IReadModel
        {
            throw new NotImplementedException();
        }
    }
}