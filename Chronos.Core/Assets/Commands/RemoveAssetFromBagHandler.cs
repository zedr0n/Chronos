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
        
        public void Handle(RemoveAssetFromBagCommand command)
        {
            var bag = _domainRepository.Get<Bag>(command.TargetId);
            bag.Remove(command.AssetId,command.Quantity);
            _domainRepository.Save(bag);
        }
    }
}
