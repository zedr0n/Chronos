using System;
using Chronos.Infrastructure;
using Chronos.Infrastructure.Commands;

namespace Chronos.Core.Accounts.Commands
{
    public class DepositAssetHandler : CommandHandlerBase,ICommandHandler<DepositAssetCommand>
    {
        public void Handle(DepositAssetCommand command)
        {
            var account = Repository.Find<Account>(command.AggregateId);

            account.DepositAsset(command.AssetId);
            Repository.Save(account);
        }

        public DepositAssetHandler(IDomainRepository domainRepository) : base(domainRepository)
        {
        }
    }
}