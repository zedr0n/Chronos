using System;
using Chronos.Infrastructure;
using Chronos.Infrastructure.Commands;

namespace Chronos.Core.Assets.Commands
{
	public class UpdateAssetPriceHandler : ICommandHandler<UpdateAssetPriceCommand>
	{
		private readonly IDomainRepository _domainRepository;

		public UpdateAssetPriceHandler(IDomainRepository domainRepository)
		{
			_domainRepository = domainRepository;
		}

		public void Handle(UpdateAssetPriceCommand command)
		{
			var asset = _domainRepository.Find<IAsset>(command.TargetId);
			if(asset == null)
				throw new InvalidOperationException("Asset does not exist");
				
			asset.UpdatePrice(command.Price);
			_domainRepository.Save(asset);
		}
	}

    
}