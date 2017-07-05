using System;
using Chronos.Infrastructure;
using Chronos.Infrastructure.Commands;

namespace Chronos.Core.Account.Commands
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
        /// <para> @<see cref="Aggregate"/>:<see cref="Changed"/>! -> </para>
        /// <para> -> <see cref="Aggregate.OnAccountChanged"/> </para>
        /// <para> -> AccountInfoViewModel::OnAccountChanged</para>
        /// <para> @<see cref="Aggregate"/>:<see cref="Created"/>! -> CreateAccountViewModel::OnAccountCreated </para>
        /// </summary>
        public void Handle(ChangeAccountCommand command)
        {
            //var account = _repository.GetById(command.Guid);
            var account = _domainRepository.Find<Aggregate>(command.Guid);
            if(account == null)
                throw new InvalidOperationException("Account does not exist");
           
            account.ChangeDetails(command.Name, command.Currency);

            _domainRepository.Save(account);
        }
    }
}