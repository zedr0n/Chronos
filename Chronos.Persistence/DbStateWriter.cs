using System;
using System.Linq;
using System.Reflection;
using Chronos.Infrastructure;
using Chronos.Infrastructure.Logging;
using Chronos.Infrastructure.Projections;

namespace Chronos.Persistence
{
    public class DbStateWriter : IStateWriter
    {
        private readonly IReadDb _db;
        private readonly IMemoryStateWriter _stateWriter;
        private readonly IDebugLog _debugLog;
        private double _writeTime = 0;
        private int _numberOfWrites = 0;
        
        public DbStateWriter(IReadDb db, IMemoryStateWriter stateWriter, IDebugLog debugLog)
        {
            _db = db;
            _stateWriter = stateWriter;
            _debugLog = debugLog;
        }

        private static bool UseMemoryProxy<T>()
        {
            return typeof(T).GetTypeInfo().GetCustomAttributes<MemoryProxyAttribute>().Any();
        }

        public void Write<TKey, T>(TKey key, Func<T,bool> action) where TKey : IEquatable<TKey> where T : class, IReadModel, new()
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
                if (!action(state))
                    return;
                //var before = _debugLog.Now();
                context.SaveChanges();
                //var after = _debugLog.Now();
                //_numberOfWrites++;
                //var duration = after - before;
                //_writeTime += duration.TotalMilliseconds; 
                //_debugLog.WriteLine("Elapsed :" + _writeTime + " for " + _numberOfWrites + " writes");
            }
        }
    }
}