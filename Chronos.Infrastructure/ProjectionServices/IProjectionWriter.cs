using System;
using System.Collections.Generic;
using Chronos.Infrastructure.Interfaces;

namespace Chronos.Infrastructure.ProjectionServices
{
    public interface IProjectionWriter<TKey,T> where T : IReadModel
    {
        void Write(IEnumerable<TKey> keys, IList<IEvent> events);
        IObservable<T> Models { get; }
        IObservable<bool> OpeningWindow { get; }
        IObservable<bool> ClosingWindow { get; }
    }

    public interface IStreamProjectionWriter<T> : IProjectionWriter<Guid,T> where T : IReadModel
    {
        
    }
}