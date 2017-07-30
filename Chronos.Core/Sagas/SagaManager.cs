using System.Diagnostics;
using Chronos.Core.Transactions.Events;
using Chronos.Infrastructure;
using Chronos.Infrastructure.Events;
using Chronos.Infrastructure.Logging;

namespace Chronos.Core.Sagas
{
    public class SagaManager : SagaManagerBase,
        IConsumer<PurchaseCreated>,
        IConsumer<CommandScheduled>,
        IConsumer<TimeoutCompleted>,
        IConsumer<CashTransferCreated>
    {

        [DebuggerStepThrough]
        public void When(PurchaseCreated e) => When<PurchaseCreated,TransactionSaga>(e, x => x.PurchaseId);
        [DebuggerStepThrough]
        public void When(CommandScheduled e) => When<CommandScheduled, SchedulerSaga>(e, x => x.ScheduleId);
        [DebuggerStepThrough]
        public void When(TimeoutCompleted e) => When<TimeoutCompleted, SchedulerSaga>(e, x => x.ScheduleId);
        [DebuggerStepThrough]
        public void When(CashTransferCreated e) => When<CashTransferCreated, TransferSaga>(e, x => x.TransactionId);

        public SagaManager(ISagaRepository repository, IEventBus eventBus, IDebugLog debugLog) : base(repository, eventBus, debugLog)
        {
        }
    }
}