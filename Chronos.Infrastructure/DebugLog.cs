using System.Diagnostics;

namespace Chronos.Infrastructure
{
    public class DebugLog : IDebugLog
    {
        public void Write(string message)
        {
            Debug.Write(message);
        }

        public void WriteLine(string message)
        {
            Debug.WriteLine(message);
        }
    }
}