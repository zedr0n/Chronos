using Chronos.Infrastructure.Interfaces;

namespace Chronos.Infrastructure
{
    public interface IReadModel
    {
        void When(IEvent e);
    }

    public interface IReadModel<TKey> : IReadModel
    {
        TKey Key { get; set; }
    }
}