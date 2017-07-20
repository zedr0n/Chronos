namespace Chronos.Infrastructure
{
    public interface IReadModel { }
    public interface IReadModel<TKey> : IReadModel
    {
        TKey Key { get; set; }
    }
}