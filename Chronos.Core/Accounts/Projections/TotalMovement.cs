﻿using System;
using Chronos.Core.Accounts.Events;
using Chronos.Infrastructure;

namespace Chronos.Core.Accounts.Projections
{
    /// <summary>
    /// Total cash movement over all accounts
    /// </summary>
    public class TotalMovement : ReadModelBase<Guid>
    {
        public double Value { get; private set; }

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