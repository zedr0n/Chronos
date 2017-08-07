namespace Chronos.Infrastructure.Queries
{
    public interface IQueryProcessor<out TResult>
        where TResult : class, IReadModel, new()
    {
         TResult Handle(IQuery query);
    }
}