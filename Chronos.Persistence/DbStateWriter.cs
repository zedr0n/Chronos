using System;
using Chronos.Infrastructure;
using Chronos.Infrastructure.Projections.New;

namespace Chronos.Persistence
{
    public class DbStateWriter : IStateWriter
    {
        private readonly IReadDb _db;

        public DbStateWriter(IReadDb db)
        {
            _db = db;
        }

        public void Write<TKey, T>(TKey key, Action<T> action) where TKey : IEquatable<TKey> where T : class, IReadModel, new()
        {
            using (var context = _db.GetContext())
            {
                var state = context.Set<T>().Find(key);
                if (state == null)
                {
                    state = new T();
                    ((IReadModel<TKey>)state).Key = key;
                    context.Set<T>().Add(state);
                }
                action(state);
                context.SaveChanges();
            }
        }
    }
}