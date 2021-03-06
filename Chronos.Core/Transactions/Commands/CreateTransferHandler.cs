﻿using System;
using Chronos.Core.Assets;
using Chronos.Core.Common;
using Chronos.Infrastructure;
using Chronos.Infrastructure.Commands;

namespace Chronos.Core.Transactions.Commands
{
    public class CreateTransferHandler : ICommandHandler<CreateCashTransferCommand>
    {
        private readonly IDomainRepository _domainRepository;

        public CreateTransferHandler(IDomainRepository domainRepository)
        {
            _domainRepository = domainRepository;
        }

        public void Handle(CreateCashTransferCommand command)
        {
            //if (_domainRepository.Exists<CashTransfer>(command.TargetId))
            //    throw new InvalidOperationException("Transaction already exists");

            var transfer = new CashTransfer(command.TargetId, command.FromAccount,command.ToAccount,
                new Cash(command.Currency,command.Amount));

            _domainRepository.Save(transfer);
        }
    }
}