using Chronos.Infrastructure;
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
            Output?.WriteLine(message);
        }
    }
}