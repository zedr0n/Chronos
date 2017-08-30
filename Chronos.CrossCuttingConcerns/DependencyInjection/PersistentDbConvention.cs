using System.Linq.Expressions;
using SimpleInjector;

namespace Chronos.CrossCuttingConcerns.DependencyInjection
{
    public class PersistentDbConvention : BaseParameterConvention
    {
        private readonly bool _persistentDb;

        public PersistentDbConvention(bool persistentDb)
        {
            _persistentDb = persistentDb;
        }

        public override bool CanResolve(InjectionTargetInfo target) => target.TargetType == typeof(bool) && target.Name == "isPersistent";

        public override Expression BuildExpression(InjectionConsumerInfo consumer)
        {
            return Expression.Constant(_persistentDb, typeof(bool));
        }
    }
}