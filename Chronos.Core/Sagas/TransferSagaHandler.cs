using Chronos.Core.Transactions.Events;
using Chronos.Infrastructure;
using Chronos.Infrastructure.Logging;
using Chronos.Infrastructure.Sagas;

namespace Chronos.Core.Sagas
{
    public class TransferSagaHandler : SagaHandlerBase<TransferSaga>
    {
        public TransferSagaHandler(ISagaRepository repository, IDebugLog debugLog, IEventStore eventStore,
            ISagaEventHandler eventHandler) 
            : base(repository, debugLog, eventStore,eventHandler)
        {
            Register<CashTransferCreated>(e => e.TransferId);
        }
    }
}