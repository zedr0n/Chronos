using Chronos.Core.Net.Tracking.Commands;
using Chronos.Infrastructure.Commands;

namespace Chronos.Console
{
    public class StopTrackingListener : ChronosBaseListener
    {
        private readonly ICommandBus _commandBus;

        public StopTrackingListener(ICommandBus commandBus)
        {
            _commandBus = commandBus;
        }

        public override void EnterStop(ChronosParser.StopContext context)
        {
            var command = new StopTrackingCommand();
            _commandBus.Send(command);
            base.EnterStop(context);
        }
    }
}