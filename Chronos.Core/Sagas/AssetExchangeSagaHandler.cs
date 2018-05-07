using Chronos.Infrastructure;
using Chronos.Infrastructure.Logging;
using Chronos.Infrastructure.Sagas;

namespace Chronos.Core.Sagas
{
    public class AssetExchangeSagaHandler : SagaHandlerBase<AssetExchangeSaga>
    {
        protected AssetExchangeSagaHandler(ISagaRepository repository, IDebugLog debugLog, IEventStore eventStore, ISagaEventHandler eventHandler) : base(repository, debugLog, eventStore, eventHandler)
        {
        }
    }
}