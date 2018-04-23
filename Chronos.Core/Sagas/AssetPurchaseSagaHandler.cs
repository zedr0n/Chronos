using Chronos.Core.Assets.Events;
using Chronos.Core.Coinbase.Events;
using Chronos.Infrastructure;
using Chronos.Infrastructure.Logging;
using Chronos.Infrastructure.Sagas;

namespace Chronos.Core.Sagas
{
    public class AssetPurchaseSagaHandler : SagaHandlerBase<AssetPurchaseSaga>
    {
        public AssetPurchaseSagaHandler(ISagaRepository repository, 
            IDebugLog debugLog, 
            IEventStore eventStore,
            ISagaEventHandler eventHandler) : base(repository, debugLog, eventStore, eventHandler)
        {
            Register<CoinPurchased>( e => e.PurchaseId);
            Register<AssetPurchased>(e => e.PurchaseId);
        }
     }
    
}