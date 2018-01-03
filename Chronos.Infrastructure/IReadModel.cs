using System;
using Chronos.Infrastructure.Interfaces;

namespace Chronos.Infrastructure
{
    public interface IReadModel
    {
        int Version { get; set; }
        bool When(IEvent e);
        Guid Timeline { get; set; }

        IReadModel Clone();
    }

    public interface IReadModel<TKey> : IReadModel
    {
        TKey Key { get; set; }
    }
}