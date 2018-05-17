using System;
using Chronos.Core.Assets.Events;
using Chronos.Infrastructure;
using Chronos.Infrastructure.Interfaces;

namespace Chronos.Core.Assets
{
    public class AssetExchange : AggregateBase
    {
        public AssetExchange(Guid fromAsset, Guid toAsset, double fromQuantity, double toQuantity,Guid exchangeId)
        {
            When(new AssetExchanged(fromAsset,toAsset, fromQuantity, toQuantity,exchangeId));
        }

        public void When(AssetExchanged e)
        {
            base.When(e);
        }

        protected override void When(IEvent e)
        {
            When((dynamic) e);
        }
    }
}