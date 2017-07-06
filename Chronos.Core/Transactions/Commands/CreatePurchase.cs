using System;
using Chronos.Infrastructure.Commands;
using NodaTime;

namespace Chronos.Core.Transactions.Commands
{
    public class CreatePurchaseCommand : ICommand
    {
        public Guid Id { get; set; }
        public Guid AccountId { get; set; }
        public string Payee { get; set; }
        public string Currency { get; set; }
        public Instant Date { get; set; }
        public double Amount { get; set; }
    }
}