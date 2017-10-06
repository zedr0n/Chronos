using Chronos.Infrastructure.Interfaces;

namespace Chronos.Infrastructure
{
    public interface IReadModel
    {
        int Version { get; set; }
        void When(IEvent e);
    }

    public interface IReadModel<TKey> : IReadModel
    {
        TKey Key { get; set; }
    }
}