using Microsoft.EntityFrameworkCore;

namespace Chronos.Persistence
{
    public class EventDb : IEventDb
    {
        private readonly bool _isPersistent;
        private bool _isInitialized = false;
        private readonly string _dbName;

        public EventDb(string dbName, bool isPersistent)
        {
            _dbName = dbName;
            _isPersistent = isPersistent;
        }

        public void Init()
        {
            if (_isInitialized)
                return;

            using (var context = GetContext())
            {
                if (!_isPersistent)
                    context.Database.EnsureDeleted();
                context.Database.Migrate();
            }
            _isInitialized = true;
        }

        public DbContext GetContext() => new EventContext(_dbName);
    }
}