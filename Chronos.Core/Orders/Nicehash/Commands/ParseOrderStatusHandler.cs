using System.Linq;
using Chronos.Infrastructure;
using Chronos.Infrastructure.Commands;

namespace Chronos.Core.Orders.NiceHash.Commands
{
    public class ParseOrderStatusHandler : ParseJsonRequestHandler<Json.Orders,UpdateOrderStatusCommand>,
        ICommandHandler<ParseOrderStatusCommand>
    {
        public ParseOrderStatusHandler(ICommandHandler<UpdateOrderStatusCommand> handler, IJsonConnector jsonConnector) : base(handler, jsonConnector)
        {
        }

        private UpdateOrderStatusCommand Parse(ParseOrderStatusCommand command, Json.Orders data)
        {
            var orderStatus = data.Result.Orders.SingleOrDefault(x => x.Id == command.OrderNumber);
            
            var aggregateCommand = new UpdateOrderStatusCommand
            {
                TargetId = command.TargetId,
                Speed = orderStatus.Accepted_Speed,
                Spent = orderStatus.Btc_paid
            };
            return aggregateCommand;
        }


        public void Handle(ParseOrderStatusCommand command)
        {
            HandleInternal(command,data => Parse(command,data));
        }
    }
}