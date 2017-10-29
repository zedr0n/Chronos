using System;
using Chronos.Infrastructure.Interfaces;
using NodaTime;

namespace Chronos.Infrastructure
{
    public interface IReplayStrategy
    {
        void Replay(Instant date);
        Func<IMessage, bool> Replayable { get; }
    }
}