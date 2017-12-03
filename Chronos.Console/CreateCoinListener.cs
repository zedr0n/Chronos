using System;
using System.Linq;
using Chronos.Core.Assets.Commands;
using Chronos.Infrastructure.Commands;

namespace Chronos.Console
{
    public class CreateCoinListener : ChronosBaseListener
    {
        private readonly ICommandBus _commandBus;

        public CreateCoinListener(ICommandBus commandBus)
        {
            _commandBus = commandBus;
        }

        public override void EnterCreateCoin(ChronosParser.CreateCoinContext context)
        {
            var name = context.name().GetText();
            var ticker = context.ticker().GetText();
            var id = Guid.Parse(context.guidOptional().GetText());
            
            var command = new CreateCoinCommand
            {
                TargetId = id,
                Name = name, 
                Ticker = ticker 
            };
            _commandBus.Send(command);

            base.EnterCreateCoin(context);
        }
    }
}