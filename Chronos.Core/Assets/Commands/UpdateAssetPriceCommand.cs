using Chronos.Infrastructure.Commands;

namespace Chronos.Core.Assets.Commands
{
    public class UpdateAssetPriceCommand<T> : CommandBase
        where T : IAsset
    {
        public double Price { get; set; }
    }
}