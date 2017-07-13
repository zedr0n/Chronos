using System;
using Chronos.Infrastructure.Events;

namespace Chronos.Core.Accounts.Events
{
    public class AssetDeposited : EventBase
    {
        public Guid AssetId { get; set; }
    }
}