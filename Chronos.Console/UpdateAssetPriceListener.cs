using System;
using Chronos.Core.Assets.Commands;
using Chronos.Core.Assets.Projections;
using Chronos.Core.Assets.Queries;
using Chronos.Infrastructure.Commands;
using Chronos.Infrastructure.Queries;

namespace Chronos.Console
{
    public class UpdateAssetPriceListener : ChronosBaseListener
    {
        private readonly IQueryHandler<CoinInfoQuery, CoinInfo> _handler;
        private readonly ICommandBus _commandBus;

        public UpdateAssetPriceListener(ICommandBus commandBus, IQueryHandler<CoinInfoQuery, CoinInfo> handler)
        {
            _commandBus = commandBus;
            _handler = handler;
        }

        public override void EnterUpdateAssetPrice(ChronosParser.UpdateAssetPriceContext context)
        {
            var assetDescriptor = context.name().GetText();
            if (!Guid.TryParse(assetDescriptor, out var assetId))
            {
                var coinInfo = _handler.Handle(new CoinInfoQuery {Name = assetDescriptor});
                if(coinInfo == null)
                    throw new InvalidOperationException("Coin with name " + assetDescriptor + " not found");
                assetId = coinInfo.Key;
            }

            var price = double.Parse(context.price().GetText());
            
            var command = new UpdateCoinPriceCommand()
            {
                TargetId = assetId,
                Price = price
            };  
           
            _commandBus.Send(command);
            base.EnterUpdateAssetPrice(context);
        }
    }
}