using System;
using Chronos.Core.Assets;
using Chronos.Infrastructure;
using Chronos.Infrastructure.Commands;

namespace Chronos.Core.Accounts.Commands
{
    public class DepositAssetHandler : CommandHandlerBase,ICommandHandler<DepositAssetCommand>
    {
        /// <summary>
        /// <see cref="Account.DepositAsset"/>
        ///     (<see cref="DepositAssetCommand.AssetId"/>)
        ///   -> @ <see cref="Account"/> : <see cref="Chronos.Core.Accounts.Events.AssetDeposited"/>>
        /// </summary>
        /// <param name="command"></param>
        public void Handle(DepositAssetCommand command)
        {
            var account = Repository.Find<Account>(command.TargetId);

            account.DepositAsset(command.AssetId);
            Repository.Save(account);
        }

        public DepositAssetHandler(IDomainRepository domainRepository) : base(domainRepository)
        {
        }
    }
}