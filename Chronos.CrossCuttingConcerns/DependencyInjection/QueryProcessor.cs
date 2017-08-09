using System.Reflection;
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

        public TResult Process<TQuery, TResult>(HistoricalQuery<TQuery> query)
            where TQuery : IQuery
            where TResult : class, IReadModel, new()
        {
            return _container.GetInstance<IHistoricalQueryHandler<TQuery, TResult>>().Handle(query);
        }
        
        public TResult Process<TQuery, TResult>(TQuery query) 
            where TQuery : IQuery
            where TResult : class, IReadModel, new()
        {
            return _container.GetInstance<IQueryHandler<TQuery, TResult>>().Handle(query);
        }
    }
}