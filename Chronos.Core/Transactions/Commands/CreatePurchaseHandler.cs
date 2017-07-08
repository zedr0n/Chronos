using System;
using Chronos.Core.Accounts.Commands;
using Chronos.Infrastructure;
using Chronos.Infrastructure.Commands;
using NodaTime;

namespace Chronos.Core.Transactions.Commands
{
    public class CreatePurchaseHandler : ICommandHandler<Commands.CreatePurchaseCommand>
    {
        private readonly IDomainRepository _domainRepository;

        private readonly ICommandHandler<DebitAmountCommand> _debitHandler;

        public CreatePurchaseHandler(IDomainRepository domainRepository, ICommandHandler<DebitAmountCommand> debitHandler)
        {
            _domainRepository = domainRepository;
            _debitHandler = debitHandler;
        }

        public void Handle(CreatePurchaseCommand command)
        {
            var purchase = _domainRepository.Find<Purchase>(command.AggregateId);
            if(purchase != null)
                throw new InvalidOperationException("Transaction already exists");

            var account = _domainRepository.Find<Accounts.Account>(command.AccountId);
            if(account == null)
                throw new InvalidOperationException("Account does not exist");

            var purchaseInfo = new PurchaseInfo
            {
                Payee = command.Payee,
                Currency = command.Currency,
                Amount = command.Amount
            };
            purchase = new Purchase(command.AggregateId,command.AccountId,purchaseInfo);

            var debit = new DebitAmountCommand
            {
                AggregateId = command.AccountId,
                Amount = command.Amount
            };

            _debitHandler.Handle(debit);

            _domainRepository.Save(purchase);
        }
    }
}