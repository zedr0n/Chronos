namespace Chronos.Infrastructure
{
    public interface IDebugLog
    {
        void Write(string message);
        void WriteLine(string message);
    }
}