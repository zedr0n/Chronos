using Chronos.Infrastructure.Commands;

namespace Chronos.Core.Assets.Commands
{
    public abstract class UpdateAssetPriceCommand<T> : CommandBase
        where T : IAsset
    {
        /// <summary>
        /// Asset price
        /// </summary>
        public double Price { get; set; }
    }
    
    public class UpdateCoinPriceCommand : UpdateAssetPriceCommand<Coin> {}
    public class UpdateEquityPriceCommand : UpdateAssetPriceCommand<Equity> {}
}