using Microsoft.EntityFrameworkCore;

namespace Chronos.Persistence
{
    public class Context : DbContext
    {
        //private readonly string _dbName;
        //private readonly bool _inMemory;

        public Context() { }
        public Context(DbContextOptions options)
            : base(options)
        {
        }
        
        public virtual Context WithOptions(DbContextOptions options) => new Context(options);

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //if (optionsBuilder.IsConfigured)
            //    return;
            //if (_inMemory)
            //    optionsBuilder.UseInMemoryDatabase(_dbName);
            //else
            //    optionsBuilder.UseSqlite(@"Filename=" + _dbName);
        }
    }
}