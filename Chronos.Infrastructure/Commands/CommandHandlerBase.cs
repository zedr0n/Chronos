using System;

namespace Chronos.Infrastructure.Commands
{
    public abstract class CommandHandlerBase : ICommandHandler
    {
        protected IDomainRepository Repository { get; }

        protected CommandHandlerBase(IDomainRepository domainRepository)
        {
            Repository = domainRepository;
        }

        protected void Act<T>(Guid id, Action<T> action) where T : class, IAggregate
        {
            var aggregate = Repository.Find<T>(id);
            action(aggregate);
            Repository.Save(aggregate);
        }
    }
}