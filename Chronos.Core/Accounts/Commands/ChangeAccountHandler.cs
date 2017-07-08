using System;
using Chronos.Core.Accounts.Events;
using Chronos.Infrastructure;
using Chronos.Infrastructure.Commands;

namespace Chronos.Core.Accounts.Commands
{
    public class ChangeAccountHandler : ICommandHandler<ChangeAccountCommand>
    {
        private readonly IDomainRepository _domainRepository;

        public ChangeAccountHandler(IDomainRepository domainRepository)
        {
            _domainRepository = domainRepository;
        }

        /// <summary>
        /// <see cref="ChangeAccountCommand"/>? -> <see cref="ChangeAccountHandler"/> 
        /// <para> @<see cref="Account"/>:<see cref="AccountChanged"/>!</para>
        /// <para> -> <see cref="Account.OnAccountChanged"/> </para>
        /// </summary>
        public void Handle(ChangeAccountCommand command)
        {
            //var account = _repository.GetById(command.Guid);
            var account = _domainRepository.Find<Account>(command.AggregateId);
            if(account == null)
                throw new InvalidOperationException("Account does not exist");
           
            account.ChangeDetails(command.Name, command.Currency);

            _domainRepository.Save(account);
        }
    }
}