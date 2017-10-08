using Chronos.Persistence.Types;
using Microsoft.EntityFrameworkCore;

namespace Chronos.Persistence.Contexts
{
    public class EventContext : Context
    {
        public EventContext() { }
        public EventContext(DbContextOptions options)
            : base(options) { }

        public override Context WithOptions(DbContextOptions options) => new EventContext(options);

        public DbSet<Event> Events { get; set; }
        public DbSet<Stream> Streams { get; set; }
        public DbSet<Command> Commands { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Stream>().HasKey(x => new {x.HashId, x.TimelineId});
        }
        
        public virtual void Clear()
        {
            Events.RemoveRange(Events);
            Commands.RemoveRange(Commands);
            SaveChanges();
        }
    }
}