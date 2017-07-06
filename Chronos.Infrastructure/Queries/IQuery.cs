namespace Chronos.Infrastructure.Queries
{
    public interface IQuery { }

    public interface IQuery<TResult> : IQuery
    {
    }
}