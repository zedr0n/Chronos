using System;
using Chronos.Infrastructure.Commands;

namespace Chronos.Core.Scheduling.Commands
{
    public class CancelTimeoutCommand : CommandBase
    {
        public Guid ScheduleId { get; set; }    
    }
}