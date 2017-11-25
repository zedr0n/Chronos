using System;
using Chronos.Infrastructure.Events;

namespace Chronos.Core.Assets.Events
{
    public class BagCreated : EventBase
    {
        public BagCreated(Guid bagId)
        {
            BagId = bagId;
        }

        public Guid BagId { get; }
    }
}