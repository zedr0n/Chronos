using Chronos.Core.Assets.Projections;
using Chronos.Core.Assets.Queries;
using Chronos.Core.Net.Tracking.Commands;
using Chronos.Infrastructure.Commands;
using Chronos.Infrastructure.Queries;
using NodaTime;

namespace Chronos.Console
{
    public class TrackCoinListener : ChronosBaseListener
    {
        private readonly ICommandBus _commandBus;
        private readonly IQueryHandler<CoinInfoQuery,CoinInfo> _handler;

        public TrackCoinListener(IQueryHandler<CoinInfoQuery, CoinInfo> handler, ICommandBus commandBus)
        {
            _handler = handler;
            _commandBus = commandBus;
        }

        public override void EnterTrackCoin(ChronosParser.TrackCoinContext context)
        {
            var name = context.name().GetText();
            var coinInfo = _handler.Handle(new CoinInfoQuery
            {
                Name = name
            });

            if (coinInfo != null)
            {
                var command = new TrackCoinCommand(coinInfo.Key, 
                        Duration.FromSeconds(int.Parse(context.duration().GetText())))
                    {
                        Ticker = name
                    };
                _commandBus.Send(command);
                _commandBus.Send(new StartTrackingCommand());
            }
            
            base.EnterTrackCoin(context);
        }
    }
}