using System;
using Chronos.Infrastructure;
using Chronos.Infrastructure.Commands;

namespace Chronos.Core.Assets.Commands
{
	public class UpdateAssetPriceHandler<T> : ICommandHandler<UpdateAssetPriceCommand<T>> where T : class,IAsset,new()
	{
		private readonly IDomainRepository _domainRepository;

		public UpdateAssetPriceHandler(IDomainRepository domainRepository)
		{
			_domainRepository = domainRepository;
		}

		public void Handle(UpdateAssetPriceCommand<T> command)
		{
			var asset = _domainRepository.Find<T>(command.TargetId);
			if(asset == null)
				throw new InvalidOperationException("Asset does not exist");
				
			asset.UpdatePrice(command.Price);
			_domainRepository.Save(asset);
		}
	}

    
}