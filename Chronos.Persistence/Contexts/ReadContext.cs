using Chronos.Core.Accounts.Projections;
using Chronos.Core.Assets.Projections;
using Chronos.Core.Nicehash.Projections;
using Chronos.Core.Projections;
using Microsoft.EntityFrameworkCore;

namespace Chronos.Persistence.Contexts
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
        public DbSet<CoinInfo> Coins { get; set; }
        public DbSet<OrderInfo> Orders { get; set; }
        public DbSet<OrderStatus> OrderStatuses { get; set; }
        
        public virtual void Clear()
        {
            Accounts.RemoveRange(Accounts);
            Movements.RemoveRange(Movements);
            Stats.RemoveRange(Stats); 
        }
    }
}