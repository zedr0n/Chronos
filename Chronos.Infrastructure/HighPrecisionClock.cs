using System;
using NodaTime;

namespace Chronos.Infrastructure
{
    public class HighPrecisionClock : IClock
    {
        public Instant GetCurrentInstant()
        {
            if (HighResolutionDateTime.IsAvailable)
                return NodaConstants.BclEpoch.PlusTicks(HighResolutionDateTime.UtcNow.Ticks);
            else
                return NodaConstants.BclEpoch.PlusTicks(DateTime.UtcNow.Ticks);
        }
    }
}