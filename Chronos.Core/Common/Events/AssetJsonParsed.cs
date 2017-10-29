using System;
using Chronos.Infrastructure.Events;

namespace Chronos.Core.Common.Events
{
    public class AssetJsonParsed : EventBase
    {
        protected AssetJsonParsed(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; } 
    }
}