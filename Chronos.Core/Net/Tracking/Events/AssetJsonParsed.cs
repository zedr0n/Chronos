using System;
using Chronos.Infrastructure.Events;

namespace Chronos.Core.Net.Tracking.Events
{
    public class AssetJsonParsed : EventBase
    {
        public virtual bool Finish() => true;
        
        protected AssetJsonParsed(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; } 
    }
}