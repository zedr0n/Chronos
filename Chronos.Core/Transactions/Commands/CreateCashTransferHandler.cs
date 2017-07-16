using System;
using Chronos.Infrastructure;
using Chronos.Infrastructure.Commands;

namespace Chronos.Core.Transactions.Commands
{
    public class CreateCashTransferHandler : CommandHandlerBase, ICommandHandler<CreateCashTransferCommand>
    {
        public CreateCashTransferHandler(IDomainRepository domainRepository) : base(domainRepository)
        {
        }

        public void Handle(CreateCashTransferCommand command)
        {
            if(Repository.Exists<CashTransfer>(command.AggregateId))
                throw new InvalidOperationException("Transaction already exists");

            var transfer = new CashTransfer(command.AggregateId,command.FromAccount,command.ToAccount, new Cash(command.Currency,command.Amount));
            Repository.Save(transfer);
        }
    }
}