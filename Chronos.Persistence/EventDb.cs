using Microsoft.EntityFrameworkCore;

namespace Chronos.Persistence
{
    public class EventDb : IEventDb
    {
        private readonly bool _isPersistent;
        private readonly bool _inMemory;
        private bool _isInitialized = false;
        private readonly string _dbName;
        private readonly object _lock;

        public EventDb(string dbName, bool isPersistent, bool inMemory)
        {
            _dbName = dbName;
            _isPersistent = isPersistent;
            _inMemory = inMemory;
            _lock = new object();
            Init();
        }

        public void Init()
        {
            if (_inMemory)
                return;
            lock (_lock)
            {
                if (_isInitialized)
                    return;

                using (var context = GetContext())
                {
                    if (!_isPersistent)
                        context.Database.EnsureDeleted();
                    context.Database.Migrate();
                    //context.LogToConsole();
                }
                _isInitialized = true;
            }
        }

        public DbContext GetContext() => new EventContext(_dbName,_inMemory);
    }
}