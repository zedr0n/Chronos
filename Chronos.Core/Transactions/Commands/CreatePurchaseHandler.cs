using System;
using Chronos.Infrastructure;
using Chronos.Infrastructure.Commands;
using NodaTime;

namespace Chronos.Core.Transactions.Commands
{
    public class CreatePurchaseHandler : ICommandHandler<CreatePurchaseCommand>
    {
        private readonly IDomainRepository _domainRepository;

        public CreatePurchaseHandler(IDomainRepository domainRepository)
        {
            _domainRepository = domainRepository;
        }

        public void Handle(CreatePurchaseCommand command)
        {
            if(_domainRepository.Exists<Purchase>(command.AggregateId))
                throw new InvalidOperationException("Transaction already exists");

            if(!_domainRepository.Exists<Accounts.Account>(command.AccountId))
                throw new InvalidOperationException("Account doesn't exist");

            var purchaseInfo = new PurchaseInfo
            {
                Payee = command.Payee,
                Currency = command.Currency,
                Amount = command.Amount
            };
            var purchase = new Purchase(command.AggregateId,command.AccountId,purchaseInfo);
            _domainRepository.Save(purchase);
        }
    }
}