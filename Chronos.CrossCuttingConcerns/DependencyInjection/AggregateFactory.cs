using System;
using System.Collections.Generic;
using System.Reflection;
using Chronos.Infrastructure;

namespace Chronos.CrossCuttingConcerns.DependencyInjection
{
    public class AggregateFactory : IAggregateFactory
    {
        private readonly Dictionary<string, Type> _aggregateTypes = new Dictionary<string, Type>();

        public AggregateFactory(List<Type> aggregateTypes)
        {
            foreach (var t in aggregateTypes)
                RegisterAggregate(t);
        }

        public void RegisterAggregate(Type type)
        {
            _aggregateTypes[type.SerializableName()] = type;
        }

        public bool Is<TInterface>(string runtimeType) where TInterface : IAggregate
        {
            if(!_aggregateTypes.ContainsKey(runtimeType))
                throw new InvalidOperationException("Aggregate type not recognized");

            var aggregateType = _aggregateTypes[runtimeType];

            if (!typeof(TInterface).IsAssignableFrom(aggregateType))
                return false;
            
            return true;
        }
        
        public TInterface Create<TInterface>(string runtimeType) where TInterface : IAggregate
        {
            if(!_aggregateTypes.ContainsKey(runtimeType))
                throw new InvalidOperationException("Aggregate type not recognized");

            var aggregateType = _aggregateTypes[runtimeType];
            
            if (!typeof(TInterface).IsAssignableFrom(aggregateType))
                return default(TInterface);

            return (TInterface) Activator.CreateInstance(aggregateType,new object[] {});
        }
    }
}