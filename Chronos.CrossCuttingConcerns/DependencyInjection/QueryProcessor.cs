using Chronos.Infrastructure;
using Chronos.Infrastructure.Queries;
using SimpleInjector;

namespace Chronos.CrossCuttingConcerns.DependencyInjection
{
    public class QueryProcessor : IQueryProcessor
    {
        private Container _container;

        public QueryProcessor(Container container)
        {
            _container = container;
        }

        public TResult Process<TQuery, TResult>(TQuery query) where TQuery : IQuery<TResult> 
            where TResult : class, IReadModel, new()
        {
            return _container.GetInstance<IQueryHandler<TQuery, TResult>>().Handle(query);
        }
    }
}