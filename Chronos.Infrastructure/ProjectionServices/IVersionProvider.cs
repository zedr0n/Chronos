namespace Chronos.Infrastructure.ProjectionServices
{
    public interface IVersionProvider<T> where T : IReadModel
    {
        int Get(StreamDetails s);
    }
}