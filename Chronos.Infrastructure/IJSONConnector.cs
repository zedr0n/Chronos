using System;
using System.Threading.Tasks;

namespace Chronos.Infrastructure
{
    public interface IJsonConnector
    {
        void SubmitRequest(string url);
        IObservable<Lazy<IObservable<string>>> GetRequest(string url);
    }
}