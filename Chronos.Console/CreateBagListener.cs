using System;
using Chronos.Core.Assets.Commands;
using Chronos.Infrastructure.Commands;

namespace Chronos.Console
{
    public class CreateBagListener : ChronosBaseListener
    {
        private readonly ICommandBus _commandBus;

        public CreateBagListener(ICommandBus commandBus)
        {
            _commandBus = commandBus;
        }

        public override void EnterCreateBag(ChronosParser.CreateBagContext context)
        {
            var id = Guid.Parse(context.guidOptional().GetText());
            var command = new CreateBagCommand(context.name().GetText())
            {
                TargetId = id
            };
            _commandBus.Send(command);
            base.EnterCreateBag(context);
        }
    }
}