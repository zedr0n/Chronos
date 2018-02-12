using System;

namespace Chronos.Infrastructure.ProjectionServices
{
    public interface IStreamSelector<T> where T : IReadModel
    {
        IObservable<StreamDetails> Streams { get; }
    }
}