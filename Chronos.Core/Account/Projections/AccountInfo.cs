using Chronos.Infrastructure.Projections;

namespace Chronos.Core.Account.Projections
{
    public class AccountInfo : Projection
    {
        public string Name { get; set; }
        public string Currency { get; set; }
        public double Balance { get; set; }
    }
}