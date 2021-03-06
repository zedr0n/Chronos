﻿using System;
using Chronos.Infrastructure.Events;

namespace Chronos.Core.Accounts.Events
{
    public class AssetWithdrawn : EventBase
    {
        public Guid AccountId { get; set; }
        public Guid AssetId { get; set; }
    }
}