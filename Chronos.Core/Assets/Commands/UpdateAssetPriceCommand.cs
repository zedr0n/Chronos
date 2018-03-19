using Chronos.Infrastructure.Commands;

namespace Chronos.Core.Assets.Commands
{
    public class UpdateAssetPriceCommand : CommandBase
    {
        /// <summary>
        /// Asset price
        /// </summary>
        public double Price { get; set; }
    }
}