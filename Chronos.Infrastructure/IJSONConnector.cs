using System;

namespace Chronos.Infrastructure
{
    public interface IJsonConnector
    {
        T Get<T>(string url)
            where T : class;

        void Save<T>(Guid requestId, T result)
            where T : class;

        T Find<T>(Guid requestId)
            where T : class;
    }
}