using System;
using NodaTime;

namespace Chronos.Infrastructure
{
    public interface ITimeline
    {
        // id of the alternate timeline we are in
        // at the moment
        // Empty if live
        Guid TimelineId { get; }
        
        bool Live { get; }
        Instant Now();
        
        void Set(Instant date);
        void Reset();
        
        IObservable<T> StopAt<T>(Instant date, T value);
        void Alternate(Guid timelineId);
    }
}