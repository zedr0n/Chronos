using System;
using Chronos.Infrastructure;
using Chronos.Infrastructure.Projections;
using Chronos.Core.Accounts.Events;
using NodaTime;

namespace Chronos.Core.Accounts.Projections
{
    /// <summary>
    /// Account state read model
    /// </summary>
    public class AccountInfo : ReadModelBase<Guid>
    {
        public string Name { get; private set; }
        public string Currency { get; private set; }
        public double Balance { get; private set; }
        public Instant CreatedAt { get; private set; }

        private void When( AccountCreated e)
        {
            Name = e.Name;
            Currency = e.Currency;
            Balance = 0;
            CreatedAt = e.Timestamp;
        }
        private void When(AccountChanged e)
        {
            Name = e.Name;
            Currency = e.Currency;
        }
        private void When( CashDeposited e)
        {
            Balance += e.Amount;
        }
        private void When( CashWithdrawn e )
        {
            Balance -= e.Amount;
        }
    }
}