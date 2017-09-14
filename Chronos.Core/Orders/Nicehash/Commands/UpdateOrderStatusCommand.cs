using Chronos.Infrastructure.Commands;

namespace Chronos.Core.Orders.NiceHash.Commands
{
    public class UpdateOrderStatusCommand : CommandBase
    {
        public double Speed { get; set; }
        public double Spent { get; set; }
    }
}