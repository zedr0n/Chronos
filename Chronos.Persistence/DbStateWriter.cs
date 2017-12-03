using System;
using System.Linq;
using System.Reflection;
using Chronos.Infrastructure;
using Chronos.Infrastructure.Projections.New;

namespace Chronos.Persistence
{
    public class DbStateWriter : IStateWriter
    {
        private readonly IReadDb _db;
        private readonly IMemoryStateWriter _stateWriter;

        public DbStateWriter(IReadDb db, IMemoryStateWriter stateWriter)
        {
            _db = db;
            _stateWriter = stateWriter;
        }

        private bool UseMemoryProxy<T>()
        {
            return typeof(T).GetTypeInfo().GetCustomAttributes<MemoryProxyAttribute>().Any();
        }

        public void Write<TKey, T>(TKey key, Action<T> action) where TKey : IEquatable<TKey> where T : class, IReadModel, new()
        {
            using (var context = _db.GetContext())
            {
                T state;
                
                if (UseMemoryProxy<T>())
                {
                    state = _stateWriter.GetState<TKey, T>(key);

                    if (state != null)
                        context.Set<T>().Attach(state);
                    else // state might exists in db even when memory instance was not created yet
                    {
                        state = context.Set<T>().Find(key);
                        _stateWriter.SetState(key, state);
                    }
                }
                else
                    state = context.Set<T>().Find(key);
                
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