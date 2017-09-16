using Chronos.Infrastructure.Commands;

namespace Chronos.Core.Orders.NiceHash.Commands
{
    public class ParseOrderStatusCommand : ParseJsonRequestCommand
    {
        public int OrderNumber { get; set; }
    }
}