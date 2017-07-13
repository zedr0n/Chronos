using Chronos.Infrastructure.Events;

namespace Chronos.Core.Transactions.Events
{
    public class CashTransferCreated : TransferCreated
    {
        public string Currency { get; set; }
        public double Amount { get; set; }
    }
}