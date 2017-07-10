﻿using System;
using Chronos.Infrastructure.Commands;
using NodaTime;

namespace Chronos.Core.Transactions.Commands
{
    public class ScheduleCommand : CommandBase
    {
        public Guid ScheduleId { get; set; }
        public ICommand Command { get; set; }
        public Instant Date { get; set; }
    }
}