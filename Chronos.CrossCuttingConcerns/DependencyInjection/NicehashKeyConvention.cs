using System.Linq.Expressions;
using SimpleInjector;

namespace Chronos.CrossCuttingConcerns.DependencyInjection
{
    public class NicehashKeyConvention : BaseParameterConvention
    {
        private readonly string _key = "";
        private readonly int _id = 0; 
        
        public override bool CanResolve(InjectionTargetInfo target)
        {
            return target.TargetType == typeof(string)
                   && target.Name == "key"
                   || target.TargetType == typeof(int)
                   && target.Name == "id";
        }

        public override Expression BuildExpression(InjectionConsumerInfo consumer)
        {
            if(consumer.Target.Name == "key")
                return Expression.Constant(_key, typeof(string));
            
            return Expression.Constant(_id,typeof(int));
        }
    }
}