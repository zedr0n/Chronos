using Chronos.Infrastructure;
using Chronos.Infrastructure.Commands;

namespace Chronos.Core.Assets.Commands
{
    public class RemoveAssetFromBagHandler : ICommandHandler<RemoveAssetFromBagCommand>
    {
        private readonly IDomainRepository _domainRepository;
        
        public RemoveAssetFromBagHandler(IDomainRepository domainRepository)
        {
            _domainRepository = domainRepository;
        }
        
        /// <summary>
        /// <see cref="Bag.Remove"/>
        ///   (<see cref="RemoveAssetFromBagCommand.AssetId"/>,<see cref="RemoveAssetFromBagCommand.Quantity"/>)
        ///   -> @ <see cref="Bag"/> : <see cref="Chronos.Core.Assets.Events.AssetRemovedFromBag"/>
        /// </summary>
        /// <param name="command"></param>
        public void Handle(RemoveAssetFromBagCommand command)
        {
            var bag = _domainRepository.Get<Bag>(command.TargetId);
            bag.Remove(command.AssetId,command.Quantity);
            _domainRepository.Save(bag);
        }
    }
}
