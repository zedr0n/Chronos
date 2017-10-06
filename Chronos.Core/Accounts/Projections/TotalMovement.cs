using System;
using Chronos.Core.Accounts.Events;
using Chronos.Infrastructure;
using Chronos.Infrastructure.Events;

namespace Chronos.Core.Accounts.Projections
{
    /// <summary>
    /// Total cash movement over all accounts
    /// </summary>
    public class TotalMovement : ReadModelBase<Guid>
    {
        public double Value { get; private set; }

        private void When(StateReset e)
        {
            Value = 0;
        }
        
        private void When(CashDeposited e)
        {
            Value += Math.Abs(e.Amount);
        }

        private void When(CashWithdrawn e)
        {
            Value += Math.Abs(e.Amount);
        }
    }
}