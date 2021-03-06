﻿using System;
using Chronos.Infrastructure.Events;

namespace Chronos.Core.Nicehash.Events
{
    public class NicehashOrderUpdated : EventBase
    {
        public Guid OrderId { get; set; }
        public double Spent { get; set; }
        public double Speed { get; set; }
    }
}