using Chronos.Infrastructure;

namespace Chronos.Core.Assets
{
    public interface IAsset : IAggregate
    {
        void UpdatePrice(double price);
    }
}