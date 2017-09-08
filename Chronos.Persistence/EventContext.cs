using Chronos.Persistence.Types;
using Microsoft.EntityFrameworkCore;

namespace Chronos.Persistence
{
    public class EventContext : Context
    {
        public EventContext() { }
        public EventContext(DbContextOptions options)
            : base(options) { }

        public override Context WithOptions(DbContextOptions options) => new EventContext(options);

        public DbSet<Event> Events { get; set; }
        public DbSet<Stream> Streams { get; set; }

        public override void Clear()
        {
            Events.RemoveRange(Events);
            SaveChanges();
        }
    }
}