using System.Linq.Expressions;
using SimpleInjector;

namespace Chronos.CrossCuttingConcerns.DependencyInjection
{
    public class InMemoryConvention : BaseParameterConvention
    {
        private readonly bool _inMemory;

        public InMemoryConvention(bool inMemory)
        {
            _inMemory = inMemory;
        }

        public override bool CanResolve(InjectionTargetInfo target) => target.TargetType == typeof(bool) && target.Name == "inMemory";

        public override Expression BuildExpression(InjectionConsumerInfo consumer)
        {
            return Expression.Constant(_inMemory, typeof(bool));
        }
    }
}