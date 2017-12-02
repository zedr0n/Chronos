using System;
using Chronos.Core.Assets.Commands;
using Chronos.Core.Assets.Projections;
using Chronos.Core.Assets.Queries;
using Chronos.Infrastructure.Commands;
using Chronos.Infrastructure.Queries;

namespace Chronos.Console
{
    public class AddAssetToBagListener : ChronosBaseListener
    {
        private readonly IQueryHandler<CoinInfoQuery, CoinInfo> _handler;
        private readonly IQueryHandler<BagInfoQuery, BagInfo> _bagHandler;
        private readonly ICommandBus _commandBus;

        public AddAssetToBagListener(ICommandBus commandBus, IQueryHandler<CoinInfoQuery, CoinInfo> handler, IQueryHandler<BagInfoQuery, BagInfo> bagHandler)
        {
            _commandBus = commandBus;
            _handler = handler;
            _bagHandler = bagHandler;
        }

        public override void EnterAddAssetToBag(ChronosParser.AddAssetToBagContext context)
        {
            var bagDescriptor = context.bagDescriptor().GetText();
            if (!Guid.TryParse(bagDescriptor, out var id))
            {
                var bagInfo = _bagHandler.Handle(new BagInfoQuery {Name = bagDescriptor});
                if(bagInfo == null)
                    throw new InvalidOperationException("Bag with name " + bagDescriptor + " not found");
                id = bagInfo.Key;
            }
            
            var assetDescriptor = context.assetDescriptor().GetText();
            if (!Guid.TryParse(assetDescriptor, out var assetId))
            {
                var coinInfo = _handler.Handle(new CoinInfoQuery {Name = assetDescriptor});
                if(coinInfo == null)
                    throw new InvalidOperationException("Coin with name " + assetDescriptor + " not found");
                assetId = coinInfo.Key;
            }
            
            var quantity = int.Parse(context.quantity().GetText());
            
            var command = new AddAssetToBagCommand(assetId, quantity)
            {
                TargetId = id
            };
            _commandBus.Send(command);
            base.EnterAddAssetToBag(context);
        }
    }
}