using System;
using Chronos.Infrastructure;
using Chronos.Infrastructure.Logging;
using NodaTime;
using Xunit.Abstractions;

namespace Chronos.Tests
{
    public class DebugLogXUnit : IDebugLog
    {
        public ITestOutputHelper Output { private get; set; }
        public void Write(string message)
        {
            Output?.WriteLine(message);
        }

        public void WriteLine(string message)
        {
            try
            {
                Output?.WriteLine(message);
            }
            catch (Exception e)
            {
                Console.WriteLine(message);
            }
        }

        public Instant Now() => default(Instant);
    }
}