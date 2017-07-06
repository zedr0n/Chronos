using System;
using Chronos.Infrastructure.Events;
using NodaTime;

namespace Chronos.Infrastructure.Projections
{
    public interface IProjector : IConsumer
    {
        void Rebuild();
        void Rebuild(Instant upTo);
    }
}