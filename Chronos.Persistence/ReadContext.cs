using Chronos.Core.Accounts.Projections;
using Chronos.Core.Projections;
using Microsoft.EntityFrameworkCore;

namespace Chronos.Persistence
{
    public class ReadContext : Context
    {
        public ReadContext() { }
        public ReadContext(DbContextOptions options)
            : base(options) { }
       
        public override Context WithOptions(DbContextOptions options) => new ReadContext(options);
        public DbSet<AccountInfo> Accounts { get; }
        public DbSet<TotalMovement> Movements { get; }
        public DbSet<Stats> Stats { get; }
    }
}