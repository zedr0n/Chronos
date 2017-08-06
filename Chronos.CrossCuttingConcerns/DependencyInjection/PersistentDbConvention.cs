using System.Linq.Expressions;
using SimpleInjector;

namespace Chronos.CrossCuttingConcerns.DependencyInjection
{
    public class PersistentDbConvention : IParameterConvention
    {
        private readonly bool _persistentDb;

        public PersistentDbConvention(bool persistentDb)
        {
            _persistentDb = persistentDb;
        }

        public bool CanResolve(InjectionTargetInfo target) => target.TargetType == typeof(bool) && target.Name == "isPersistent";

        public Expression BuildExpression(InjectionConsumerInfo consumer)
        {
            return Expression.Constant(_persistentDb, typeof(bool));
        }
    }
}