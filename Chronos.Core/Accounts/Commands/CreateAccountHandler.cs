using System;
using Chronos.Core.Accounts.Projections;
using Chronos.Core.Accounts.Queries;
using Chronos.Infrastructure;
using Chronos.Infrastructure.Commands;
using Chronos.Infrastructure.Queries;

namespace Chronos.Core.Accounts.Commands
{
    public class CreateAccountHandler : ICommandHandler<CreateAccountCommand>
    {
        private readonly IDomainRepository _domainRepository;
        private readonly IQueryHandler<AccountInfoQuery,AccountInfo> _handler;

        public CreateAccountHandler(IDomainRepository domainRepository, IQueryHandler<AccountInfoQuery, AccountInfo> handler)
        {
            _domainRepository = domainRepository;
            _handler = handler;
        }

        /// <summary>
        /// <see cref="Account"/>
        ///   (<see cref="CreateAccountCommand.TargetId"/>,<see cref="CreateAccountCommand.Name"/>,<see cref="CreateAccountCommand.Currency"/>)
        ///   -> @ <see cref="Account"/> : <see cref="Chronos.Core.Accounts.Events.AccountCreated"/>
        /// </summary>
        public void Handle(CreateAccountCommand command)
        {
            var account = new Account(command.TargetId,command.Name, command.Currency);
            _domainRepository.Save(account);
        }
    }
}