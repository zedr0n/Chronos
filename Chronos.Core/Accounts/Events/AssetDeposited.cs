using System;
using Chronos.Infrastructure.Events;

namespace Chronos.Core.Accounts.Events
{
    public class AssetDeposited : EventBase
    {
        public Guid AccountId { get; set; }
        public Guid AssetId { get; set; }
        public double Amount { get; set; }
    }
}