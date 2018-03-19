using Chronos.Infrastructure;
using Chronos.Infrastructure.Commands;

namespace Chronos.Core.Assets.Commands
{
    public class AddAssetToBagHandler : ICommandHandler<AddAssetToBagCommand>
    {
        private readonly IDomainRepository _domainRepository;

        public AddAssetToBagHandler(IDomainRepository domainRepository)
        {
            _domainRepository = domainRepository;
        }

        /// <summary>
        /// <see cref="Bag.Add"/>
        ///   (<see cref="AddAssetToBagCommand.AssetId"/>,<see cref="AddAssetToBagCommand.Quantity"/>)
        ///  -> @ <see cref="Bag"/> : <see cref="Chronos.Core.Assets.Events.AssetAddedToBag"/>
        /// </summary>
        /// <param name="command"></param>
        public void Handle(AddAssetToBagCommand command)
        {
            var bag = _domainRepository.Get<Bag>(command.TargetId);
            bag.Add(command.AssetId,command.Quantity);
            _domainRepository.Save(bag);
        }
    }

    
}