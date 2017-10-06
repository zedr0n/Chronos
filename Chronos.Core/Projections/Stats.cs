using Chronos.Core.Accounts.Events;
using Chronos.Core.Assets.Events;
using Chronos.Infrastructure;
using Chronos.Infrastructure.Events;
using Chronos.Infrastructure.Interfaces;

namespace Chronos.Core.Projections
{
    public class Stats : ReadModelBase<string>
    {
        public int NumberOfAccounts { get; set; }
        public int NumberOfAssets { get; set; }

        public Stats()
        {
            Key = "Global";
        }
        
        public void When(AccountCreated e)
        {
            NumberOfAccounts++;
        }

        public void When(CoinCreated e)
        {
            NumberOfAssets++;
        }

        public void When(StateReset e)
        {
            NumberOfAccounts = 0;
            NumberOfAssets = 0;
        }

        public override void When(IEvent e)
        {
            base.When(e);
            // as stats are aggregated from multiple streams
            // we will always read it from scratch on load/replay
            Version = -1;
        }
    }
}