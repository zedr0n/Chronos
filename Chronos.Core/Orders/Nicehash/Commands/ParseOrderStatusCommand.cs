using Chronos.Infrastructure.Commands;

namespace Chronos.Core.Orders.NiceHash.Commands
{
    public class ParseOrderStatusCommand : ParseJsonRequestCommand<Json.Orders,UpdateOrderStatusCommand>
    {
        public int OrderNumber { get; set; }
    }
}