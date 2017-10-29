namespace Chronos.Core.Net.Tracking.Commands
{
    public interface IUrlProvider
    {
        string Get(params object[] args);
    }
}