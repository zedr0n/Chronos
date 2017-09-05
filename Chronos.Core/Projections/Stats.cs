using Chronos.Core.Accounts.Events;
using Chronos.Infrastructure;
using Chronos.Infrastructure.Events;

namespace Chronos.Core.Projections
{
    public class Stats : ReadModelBase<string>
    {
        public int NumberOfAccounts { get; set; }

        public Stats()
        {
            Key = "Global";
        }
        
        public void When(AccountCreated e)
        {
            NumberOfAccounts++;
        }

        public void When(StateReset e)
        {
            NumberOfAccounts = 0;
        }       
    }
}