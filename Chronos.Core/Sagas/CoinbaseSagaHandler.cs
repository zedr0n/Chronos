using Chronos.Core.Coinbase.Events;
using Chronos.Infrastructure;
using Chronos.Infrastructure.Logging;
using Chronos.Infrastructure.Sagas;

namespace Chronos.Core.Sagas
{
    public class CoinbaseSagaHandler : SagaHandlerBase<CoinbaseSaga>
    {
        public CoinbaseSagaHandler(ISagaRepository repository, IDebugLog debugLog, IEventStore eventStore, ISagaEventHandler eventHandler)
            : base(repository, debugLog, eventStore, eventHandler)
        {
            Register<CoinbaseAccountCreated>(e => e.AccountId);
        }
    }
}