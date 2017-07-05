﻿using System;
using Chronos.Infrastructure;
using Chronos.Infrastructure.Commands;

namespace Chronos.Core.Account.Commands
{
    public class CreateAccountHandler : ICommandHandler<CreateAccountCommand>
    {
        private readonly IDomainRepository _domainRepository;
        public CreateAccountHandler(IDomainRepository domainRepository)
        {
            _domainRepository = domainRepository;
        }

        public void Handle(CreateAccountCommand command)
        {
            var account = _domainRepository.Find<Aggregates.Account>(command.Guid);
            if(account != null)
                throw new InvalidOperationException("Account has already been created");
                
            account = new Aggregates.Account(command.Guid,command.Name, command.Currency);
            _domainRepository.Save(account);
        }
    }
}