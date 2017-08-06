using System.Linq.Expressions;
using SimpleInjector;

namespace Chronos.CrossCuttingConcerns.DependencyInjection
{
    public interface IParameterConvention
    {
        bool CanResolve(InjectionTargetInfo target);
        Expression BuildExpression(InjectionConsumerInfo consumer);
    }
}