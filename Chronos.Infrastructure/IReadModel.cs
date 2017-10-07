using System;
using Chronos.Infrastructure.Interfaces;

namespace Chronos.Infrastructure
{
    public interface IReadModel
    {
        int Version { get; set; }
        void When(IEvent e);
        Guid Timeline { get; set; }
    }

    public interface IReadModel<TKey> : IReadModel
    {
        TKey Key { get; set; }
    }
}