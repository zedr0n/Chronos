using Chronos.Infrastructure.Projections.New;

namespace Chronos.Infrastructure.Queries
{

    public interface IQueryHandler { }

    public interface IQueryHandler<in TQuery, TResult> : IQueryHandler
                                                           where TQuery : IQuery<TResult>
                                                           where TResult : class, IReadModel, new()
    {
        IProjection<TResult> Projection { get; }

        TResult Handle(TQuery query);
    }
}