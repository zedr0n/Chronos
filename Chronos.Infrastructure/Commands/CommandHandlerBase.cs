using Chronos.Infrastructure.Interfaces;

namespace Chronos.Infrastructure.Commands
{
    public abstract class CommandHandlerBase : ICommandHandler
    {
        protected IDomainRepository Repository { get; }

        protected CommandHandlerBase(IDomainRepository domainRepository)
        {
            Repository = domainRepository;
        }
    }
}