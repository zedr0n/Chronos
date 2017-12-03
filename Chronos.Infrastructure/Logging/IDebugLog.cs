using NodaTime;

namespace Chronos.Infrastructure.Logging
{
    public interface IDebugLog
    {
        
        
        void Write(string message);
        void WriteLine(string message);
        Instant Now();
    }
}