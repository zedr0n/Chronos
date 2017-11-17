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

        public void Handle(AddAssetToBagCommand command)
        {
            var bag = _domainRepository.Find<Bag>(command.TargetId);
            bag.Add(command.AssetId,command.Quantity);
            _domainRepository.Save(bag);
        }
    }

    
}