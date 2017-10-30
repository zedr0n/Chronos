using Chronos.Core.Transactions.Events;
using Chronos.Infrastructure;
using Chronos.Infrastructure.Logging;
using Chronos.Infrastructure.Sagas;

namespace Chronos.Core.Sagas
{
    public class TransactionSagaHandler : SagaHandlerBase<TransactionSaga>
    {
        public TransactionSagaHandler(ISagaRepository repository, IDebugLog debugLog, IEventStore eventStore) : base(repository, debugLog, eventStore)
        {
            Register<PurchaseCreated>(e => e.PurchaseId);
        }
    }
}