using Microsoft.EntityFrameworkCore;

namespace Chronos.Persistence
{
    public abstract class BaseDb<T> : IDb<T> where T : Context,new()
    {  
        private readonly bool _isPersistent;
        private readonly bool _inMemory; 
        private readonly string _dbName; 
        private readonly object _lock;
        private readonly DbContextOptions _options;
        
        public BaseDb(string dbName, bool isPersistent, bool inMemory)
        {
            _dbName = dbName;
            _isPersistent = isPersistent;
            _inMemory = inMemory;
            _lock = new object();
            
            var builder = new DbContextOptionsBuilder();
            if (inMemory)
                builder.UseInMemoryDatabase(dbName);
            else
                builder.UseSqlite(@"Filename=" + dbName);

             #if NET2
            #endif
            
            _options = builder.Options;
            
            if(!inMemory)
                Init();
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
    }
}