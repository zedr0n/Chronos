using Chronos.Core.Accounts.Events;
using Chronos.Infrastructure.Projections.New;

namespace Chronos.Core.Accounts.Projections
{

    public class AccountInfoHandler : ProjectionHandlerBase<AccountInfo>
    {
        public static void When(AccountInfo accountInfo, AccountCreated e)
        {
            accountInfo.Name = e.Name;
            accountInfo.Currency = e.Currency;
            accountInfo.Balance = 0;
            accountInfo.CreatedAt = e.Timestamp;
        }

        public static void When(AccountInfo accountInfo, AccountChanged e)
        {
            accountInfo.Name = e.Name;
            accountInfo.Currency = e.Currency;
        }

        public static void When(AccountInfo accountInfo, CashDeposited e)
        {
            accountInfo.Balance += e.Amount;
        }
        public static void When(AccountInfo accountInfo, CashWithdrawn e)
        {
            accountInfo.Balance -= e.Amount;
        }
    }
}