using Chronos.Infrastructure;

namespace Chronos.Core.Assets
{
    public interface IAsset : IAggregate
    {
        /// <summary>
        /// Update asset price
        /// </summary>
        void UpdatePrice(double price);
        /// <summary>
        /// Update asset change
        /// </summary>
        void UpdateChange(double hourChange, double dayChange, double weekChange);
    }
}