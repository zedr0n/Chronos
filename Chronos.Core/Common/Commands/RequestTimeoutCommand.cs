using System;
using Chronos.Infrastructure.Commands;
using NodaTime;

namespace Chronos.Core.Common.Commands
{
    public class RequestTimeoutCommand : CommandBase
    {
        public Guid ScheduleId { get; set; }
        public Duration Duration { get; set; }

        public RequestTimeoutCommand() {}
        public RequestTimeoutCommand(Guid scheduleId, Duration duration)
        {
            ScheduleId = scheduleId;
            Duration = duration;
        }
    }
}