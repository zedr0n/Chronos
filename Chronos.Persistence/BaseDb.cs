using Chronos.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Chronos.Persistence
{
    public abstract class BaseDb<T> : IDb<T> where T : Context,new()
    {  
        private readonly bool _isPersistent;
        private readonly bool _inMemory; 
        private readonly string _dbName; 
        private readonly object _lock;
        private DbContextOptions _options;

        protected BaseDb(string dbName, bool isPersistent, bool inMemory)
        {
            _dbName = dbName;
            _isPersistent = isPersistent;
            _inMemory = inMemory;
            _lock = new object();
            
            _options = Configure(_dbName, _inMemory).Options;

            if (!_inMemory)
                Init();
        }

        private DbContextOptionsBuilder Configure(string dbName, bool inMemory)
        {
            var builder = new DbContextOptionsBuilder();
            if (inMemory)
                builder.UseInMemoryDatabase(dbName);
            else
                builder.UseSqlite(@"Filename=" + dbName);

            return builder;
        }

        public void Init()
        {
            lock (_lock)
            {
                using (var context = GetContext()) 
                {
                    if (!_isPersistent)
                        context.Database.EnsureDeleted();
                    context.Database.Migrate();
                    //context.LogToConsole();
                }
            }
        }

        public DbContext GetContext() => new T().WithOptions(_options);

        public void Clear()
        {
            using(var context = GetContext())
            {
                context.Clear();
                context.SaveChanges();
            }
        }
    }
}