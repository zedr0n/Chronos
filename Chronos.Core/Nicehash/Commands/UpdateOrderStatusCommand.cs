using Chronos.Infrastructure.Commands;

namespace Chronos.Core.Nicehash.Commands
{
    public class UpdateOrderStatusCommand : CommandBase
    {
        public double Speed { get; set; }
        public double Spent { get; set; }
    }
}