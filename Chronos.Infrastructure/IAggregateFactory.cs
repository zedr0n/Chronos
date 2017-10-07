using System;

namespace Chronos.Infrastructure
{
    public interface IAggregateFactory
    {
        void RegisterAggregate(Type type);
        
        TInterface Create<TInterface>(string runtimeType)
            where TInterface : IAggregate;

        bool Is<TInterface>(string runtimeType) where TInterface : IAggregate;
    }
}