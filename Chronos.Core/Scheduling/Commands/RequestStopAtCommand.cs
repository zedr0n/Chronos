using System;
using Chronos.Infrastructure.Commands;
using NodaTime;

namespace Chronos.Core.Scheduling.Commands
{
    public class RequestStopAtCommand : CommandBase
    {
        public Guid ScheduleId { get; }    
        public Instant When { get; }
        
        public RequestStopAtCommand(Guid scheduleId, Instant when)
        {
            ScheduleId = scheduleId;
            When = when;
        }
    }
}