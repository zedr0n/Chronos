using System;
using Chronos.Core.Accounts.Events;
using Chronos.Infrastructure;
using Chronos.Infrastructure.Projections.New;

namespace Chronos.Core.Accounts.Projections.New
{
    public class AccountInfoProjection : ProjectionBase<Guid,AccountInfo>
    {
        public AccountInfoProjection(IStateWriter stateWriter, IEventStoreConnection connection) : base(stateWriter, connection)
        {
        }

        public void When(AccountInfo accountInfo, AccountCreated e)
        {
            accountInfo.Name = e.Name;
            accountInfo.Currency = e.Currency;
            accountInfo.Balance = 0;
            accountInfo.CreatedAt = e.Timestamp;
        }

        public void When(AccountInfo accountInfo, AccountChanged e)
        {
            accountInfo.Name = e.Name;
            accountInfo.Currency = e.Currency;
        }

        public void When(AccountInfo accountInfo, CashDeposited e)
        {
            accountInfo.Balance += e.Amount;
        }
        public void When(AccountInfo accountInfo, CashWithdrawn e)
        {
            accountInfo.Balance -= e.Amount;
        }
    }
}