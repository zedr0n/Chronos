using System;

namespace Chronos.Infrastructure.ProjectionServices
{
    public abstract class BaseStreamSelector<T> : IStreamSelector<T> where T : IReadModel
    {
        public abstract IObservable<StreamDetails> Streams { get; }
    }
}