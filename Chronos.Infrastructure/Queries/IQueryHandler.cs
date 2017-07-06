namespace Chronos.Infrastructure.Queries
{

    public interface IQueryHandler { }

    public interface IQueryHandler<in TQuery, out TResult> : IQueryHandler
                                                           where TQuery : IQuery<TResult>
                                                           //where TResult : class
    {
        TResult Handle(TQuery query);
    }
}