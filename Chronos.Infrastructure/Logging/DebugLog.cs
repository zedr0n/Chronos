using System;
using System.Diagnostics;
using NodaTime;

namespace Chronos.Infrastructure.Logging
{
    public class DebugLog : IDebugLog
    {
        private readonly IClock _clock;

        public DebugLog(IClock clock)
        {
            _clock = clock;
        }

        public Instant Now() => _clock.GetCurrentInstant();
        
        public void Write(string message)
        {
            //Debug.Write(message);
            Console.WriteLine(message);
        }

        public void WriteLine(string message)
        {
            //Debug.WriteLine(message);
            Console.WriteLine(message);
        }
    }
}