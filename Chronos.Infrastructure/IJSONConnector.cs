using System;
using System.Threading.Tasks;

namespace Chronos.Infrastructure
{
    public interface IJsonConnector
    {
        IObservable<string> Request(string url);
    }
}