using System;
using Chronos.Infrastructure.Events;

namespace Chronos.Core.Assets.Events
{
    public class BagCreated : EventBase
    {
        public BagCreated(Guid bagId,string name)
        {
            BagId = bagId;
            Name = name;
        }

        public Guid BagId { get; }
        public string Name { get; }
    }
}