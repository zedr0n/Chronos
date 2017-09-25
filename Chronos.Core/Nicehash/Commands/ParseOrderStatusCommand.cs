using Chronos.Infrastructure.Commands;

namespace Chronos.Core.Nicehash.Commands
{
    public class ParseOrderStatusCommand : ParseJsonRequestCommand
    {
        public int OrderNumber { get; set; }
    }
}