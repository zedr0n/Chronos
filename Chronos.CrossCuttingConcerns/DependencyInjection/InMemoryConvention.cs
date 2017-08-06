using System.Linq.Expressions;
using SimpleInjector;

namespace Chronos.CrossCuttingConcerns.DependencyInjection
{
    public class InMemoryConvention : IParameterConvention
    {
        private readonly bool _inMemory;

        public InMemoryConvention(bool inMemory)
        {
            _inMemory = inMemory;
        }

        public bool CanResolve(InjectionTargetInfo target) => target.TargetType == typeof(bool) && target.Name == "inMemory";

        public Expression BuildExpression(InjectionConsumerInfo consumer)
        {
            return Expression.Constant(_inMemory, typeof(bool));
        }
    }
}