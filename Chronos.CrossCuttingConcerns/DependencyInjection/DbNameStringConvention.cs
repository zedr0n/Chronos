using System.Linq.Expressions;
using SimpleInjector;

namespace Chronos.CrossCuttingConcerns.DependencyInjection
{
    public class DbNameStringConvention : IParameterConvention
    {
        private readonly string _dbName;

        public DbNameStringConvention(string dbName)
        {
            _dbName = dbName;
        }

        public bool CanResolve(InjectionTargetInfo target) => target.TargetType == typeof(string) && target.Name == "dbName";

        public Expression BuildExpression(InjectionConsumerInfo consumer)
        {
            return Expression.Constant(_dbName, typeof(string));
        }
    }
}