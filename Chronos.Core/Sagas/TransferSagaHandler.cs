using Chronos.Core.Sagas;
using Chronos.Core.Transactions.Events;
using Chronos.Infrastructure;
using Chronos.Infrastructure.Logging;
using Chronos.Infrastructure.Sagas;

namespace Chronos.Core.Transactions.Sagas
{
    public class TransferSagaHandler : SagaHandlerBase<TransferSaga>
    {
        public TransferSagaHandler(ISagaRepository repository, IDebugLog debugLog, IEventStore eventStore) 
            : base(repository, debugLog, eventStore)
        {
            Register<CashTransferCreated>(e => e.TransferId);
        }
    }
}