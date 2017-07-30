﻿using Chronos.Core.Assets;
using Chronos.Infrastructure;
using Chronos.Infrastructure.Commands;

namespace Chronos.Core.Accounts.Commands
{
    public class WithdrawAssetHandler : CommandHandlerBase, ICommandHandler<WithdrawAssetCommand>
    {
        public WithdrawAssetHandler(IDomainRepository domainRepository) : base(domainRepository)
        {
        }

        public void Handle(WithdrawAssetCommand command)
        {
            var account = Repository.Find<Account>(command.TargetId);

            account.WithdrawAsset(command.AssetId);
            Repository.Save(account);
        }
    }
}