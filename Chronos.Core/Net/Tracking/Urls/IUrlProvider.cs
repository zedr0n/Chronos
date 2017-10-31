namespace Chronos.Core.Net.Tracking.Urls
{
    public interface IUrlProvider
    {
        string Get(params object[] args);
    }
}