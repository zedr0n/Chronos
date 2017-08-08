using Chronos.Persistence.Types;
using Microsoft.EntityFrameworkCore;

namespace Chronos.Persistence
{
    public class EventContext : DbContext
    {
        private readonly string _dbName;
        private readonly bool _inMemory;

        public EventContext() { }
        public EventContext(string dbName, bool inMemory)
        {
            _dbName = dbName;
            _inMemory = inMemory;
        }

        public DbSet<Event> Events { get; set; }
        public DbSet<Stream> Streams { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //if (optionsBuilder.IsConfigured)
            //    return;
            if (_inMemory)
                optionsBuilder.UseInMemoryDatabase(_dbName);
            else
                optionsBuilder.UseSqlite(@"Filename=" + _dbName);
        }
    }
}