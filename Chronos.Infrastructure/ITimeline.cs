using System;
using NodaTime;

namespace Chronos.Infrastructure
{
    public interface ITimeline
    {
        bool Live { get; }
        Instant Now();
        void Set(Instant date);
        void Reset();
    }
}