using Chronos.Infrastructure;
using Chronos.Infrastructure.Commands;

namespace Chronos.Core.Assets.Commands
{
	public class PurchaseAssetHandler : ICommandHandler<PurchaseAssetCommand>
	{
		private readonly IDomainRepository _domainRepository;

		public PurchaseAssetHandler(IDomainRepository domainRepository)
		{
			_domainRepository = domainRepository;
		}

		public void Handle(PurchaseAssetCommand command)
		{
			var purchase = new AssetPurchase(command.TargetId, command.AssetId, command.Quantity, command.CostPerUnit);	
			_domainRepository.Save(purchase);
		}
	}
}
