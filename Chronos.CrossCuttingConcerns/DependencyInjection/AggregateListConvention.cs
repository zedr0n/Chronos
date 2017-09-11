using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using SimpleInjector;

namespace Chronos.CrossCuttingConcerns.DependencyInjection
{
    public class AggregateListConvention : BaseParameterConvention
    {
        private readonly List<Type> _aggregateList;

        public AggregateListConvention(List<Type> aggregateList)
        {
            _aggregateList = aggregateList;
        }

        public override bool CanResolve(InjectionTargetInfo target)
        {
            return target.TargetType == typeof(List<Type>) && target.Name == "aggregateTypes";
        }

        public override Expression BuildExpression(InjectionConsumerInfo consumer)
        {
            return Expression.Constant(_aggregateList);
        }
    }
}