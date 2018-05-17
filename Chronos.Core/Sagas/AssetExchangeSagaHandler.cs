using Chronos.Core.Assets.Events;
using Chronos.Core.Exchanges.Events;
using Chronos.Infrastructure;
using Chronos.Infrastructure.Logging;
using Chronos.Infrastructure.Sagas;

namespace Chronos.Core.Sagas
{
    public class AssetExchangeSagaHandler : SagaHandlerBase<AssetExchangeSaga>
    {
        protected AssetExchangeSagaHandler(ISagaRepository repository, IDebugLog debugLog, IEventStore eventStore, ISagaEventHandler eventHandler) : base(repository, debugLog, eventStore, eventHandler)
        {
            Register<ExchangeAdded>(e => e.ExchangeId);
            Register<ExchangeOrderFilled>(e => e.ExchangeId);
            Register<AssetExchanged>(e => e.ExchangeId);
        }
    }
}