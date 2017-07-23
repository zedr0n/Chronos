using System;
using Chronos.Core.Assets;
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

            var purchase = new Purchase(command.AggregateId,command.AccountId,command.Payee,
                new Cash(command.Currency,command.Amount));

            _domainRepository.Save(purchase);
        }
    }
}