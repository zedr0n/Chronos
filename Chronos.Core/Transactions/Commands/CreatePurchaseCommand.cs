using System;
using Chronos.Infrastructure.Commands;

namespace Chronos.Core.Transactions.Commands
{
    public class CreatePurchaseCommand : CommandBase
    {
        public Guid AccountId { get; set; }
        public string Payee { get; set; }
        public string Currency { get; set; }
        public double Amount { get; set; }
    }
}