using System;
using Chronos.Infrastructure;
using Chronos.Infrastructure.Commands;

namespace Chronos.Core.Assets.Commands
{
	public abstract class UpdateAssetPriceHandler<T> : ICommandHandler<UpdateAssetPriceCommand<T>> where T : class,IAsset,new()
	{
		private readonly IDomainRepository _domainRepository;

		public UpdateAssetPriceHandler(IDomainRepository domainRepository)
		{
			_domainRepository = domainRepository;
		}

		/// <summary>
		/// <see cref="IAsset.UpdatePrice"/>
		/// 	(<see cref="UpdateAssetPriceCommand.Price"/>)
		///   -> @ <see cref="IAsset"/> : <see cref="Chronos.Core.Assets.Events.AssetPriceUpdated"/>
		/// </summary>
		/// <param name="command"></param>
		/// <exception cref="InvalidOperationException">Asset does not exist</exception>
		public void Handle(UpdateAssetPriceCommand<T> command)
		{
			var asset = _domainRepository.Find<T>(command.TargetId);
			if(asset == null)
				throw new InvalidOperationException("Asset does not exist");
				
			asset.UpdatePrice(command.Price);
			_domainRepository.Save(asset);
		}
	}

    public class UpdateCoinPriceHandler : UpdateAssetPriceHandler<Coin>, ICommandHandler<UpdateCoinPriceCommand>
    {
	    public UpdateCoinPriceHandler(IDomainRepository domainRepository) : base(domainRepository)
	    {
	    }

	    public void Handle(UpdateCoinPriceCommand command)
	    {
		    base.Handle(command);
	    }
    }

	public class UpdateEquityPriceHandler : UpdateAssetPriceHandler<Equity>, ICommandHandler<UpdateEquityPriceCommand>
	{
		public UpdateEquityPriceHandler(IDomainRepository domainRepository) : base(domainRepository)
		{
		}

		public void Handle(UpdateEquityPriceCommand command)
		{
			base.Handle(command);
		}
	}
}