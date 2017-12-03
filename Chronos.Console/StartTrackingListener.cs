using Chronos.Core.Net.Tracking.Commands;
using Chronos.Infrastructure.Commands;

namespace Chronos.Console
{
    public class StartTrackingListener : ChronosBaseListener
    {
        private readonly ICommandBus _commandBus;

        public StartTrackingListener(ICommandBus commandBus)
        {
            _commandBus = commandBus;
        }

        public override void EnterStart(ChronosParser.StartContext context)
        {
            var command = new StartTrackingCommand();
            _commandBus.Send(command);
            base.EnterStart(context);
        }
    }
}