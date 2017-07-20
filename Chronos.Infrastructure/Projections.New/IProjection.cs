using System;
using System.Collections.Generic;
using Chronos.Infrastructure.Interfaces;
using NodaTime;

namespace Chronos.Infrastructure.Projections.New
{
    public interface IProjection<TKey,T> where T : class, new()
    {
        IProjection<TKey,T> From(IEnumerable<string> streams);
        IProjection<TKey,T> ForEachStream(Func<string, TKey> map);
        IProjection<TKey,T> ForAllStreams(TKey key);

        IProjection<TKey, T> OutputState();
        IProjection<TKey, T> AsOf(Instant date);
        void When(IEvent e);
        void When(T state, IEvent e);
        void Start();

        T State { get; }
    }
}