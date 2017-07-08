using Chronos.Core.Transactions.Events;
using Chronos.Infrastructure;
using Chronos.Infrastructure.Events;

namespace Chronos.Core.Transactions
{
    public class PurchaseInfo
    {
        public string Payee { get; set; }
        public string Currency { get; set; }
        public double Amount { get; set; }
    }
}