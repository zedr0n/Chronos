namespace Chronos.Infrastructure
{
    public interface IJSONConnector
    {
        T Get<T>(string url)
            where T : new();
    }
}