using Microsoft.EntityFrameworkCore;

namespace Chronos.Persistence
{
    public class EventContext : DbContext
    {
        private readonly string _dbName;

        public EventContext()
        {
        }

        public EventContext(string dbName)
        {
            _dbName = dbName;
        }

        public DbSet<Event> Events { get; set; }
        public DbSet<Stream> Streams { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(@"Filename=" + _dbName);
        }
    }
}