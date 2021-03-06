﻿using System;
using Chronos.Infrastructure.Interfaces;
using NodaTime;

namespace Chronos.Infrastructure.Commands
{
    public interface IHistoricalCommand {}
    
    public class HistoricalCommand<TCommand> : CommandBase,IHistoricalCommand where TCommand : class, ICommand
    {
        private Guid _targetId;
        public TCommand Command { get; set; }
        public Instant At { get; set; }

        public override Guid TargetId
        {
            get => _targetId;
            set { 
                _targetId = value;
                Command.TargetId = value;
            }
        }
    }
}