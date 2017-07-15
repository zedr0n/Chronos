using System;
using System.Runtime.InteropServices;

namespace  Chronos.Infrastructure
{

    public static class HighResolutionDateTime
    {
        private static class NativeMethods
        {
            [DllImport("Kernel32.dll", CallingConvention = CallingConvention.Winapi)]
            internal static extern void GetSystemTimePreciseAsFileTime(out long filetime);
        }

        public static bool IsAvailable { get; private set; }

        public static DateTime UtcNow
        {
            get
            {
                if (!IsAvailable)
                {
                    throw new InvalidOperationException(
                        "High resolution clock isn't available.");
                }

                NativeMethods.GetSystemTimePreciseAsFileTime(out long filetime);

                return DateTime.FromFileTimeUtc(filetime);
            }
        }

        static HighResolutionDateTime()
        {
            try
            {
                long filetime;
                NativeMethods.GetSystemTimePreciseAsFileTime(out filetime);
                IsAvailable = true;
            }
            catch (Exception)
            {
                // Not running Windows 8 or higher.
                IsAvailable = false;
            }
        }
    }

}
