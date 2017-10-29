using Chronos.Core.Common.Events;
using Chronos.Core.Nicehash.Commands;
using Chronos.Infrastructure;
using Chronos.Infrastructure.Commands;
using Chronos.Infrastructure.Handlers;

namespace Chronos.Core.Nicehash.Handlers
{
    public class OrderStatusParsedHandler : EventHandlerBase<OrderStatusParsed>
    {
        public OrderStatusParsedHandler(ICommandBus commandBus, IEventStore eventStore) : base(commandBus, eventStore)
        {
        }
        
        public override void Handle(OrderStatusParsed @event)
        {
            SendCommand(new UpdateOrderStatusCommand
            {
                TargetId = @event.OrderId,
                Speed = @event.Speed,
                Spent = @event.Spent
            });
        }
    }
}