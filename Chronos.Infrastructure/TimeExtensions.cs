using NodaTime;

namespace Chronos.Infrastructure
{
    public static class TimeExtensions
    {
        public static Instant FromSerial(double serial)
        {
            if (serial == double.MaxValue)
                return Instant.MaxValue;
            var zeroDate = Instant.FromUtc(1900, 1, 1, 0, 0);
            return zeroDate + Duration.FromDays(serial - 2);
        }

        public static double ToSerial(this Instant instant)
        {
            if (instant == default(Instant))
                return double.NaN;

            var zeroDate = Instant.FromUtc(1900, 1, 1, 0, 0);
            var duration = Duration.FromTicks((instant - zeroDate).BclCompatibleTicks);
            var toZero = duration.TotalDays + 2;
            return toZero;
        }
    }
}