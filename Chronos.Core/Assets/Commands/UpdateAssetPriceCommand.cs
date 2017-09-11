using Chronos.Infrastructure.Commands;

namespace Chronos.Core.Assets.Commands
{
    public class UpdateAssetPriceCommand : CommandBase
    {
        public double Price { get; set; }
    }
}