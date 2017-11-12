using System;
using Chronos.Infrastructure.Commands;

namespace Chronos.Core.Scheduling.Commands
{
    public class CancelTimeoutCommand : CommandBase
    {
        public CancelTimeoutCommand(Guid scheduleId)
        {
            ScheduleId = scheduleId;
        }

        public Guid ScheduleId { get; }    
    }
}