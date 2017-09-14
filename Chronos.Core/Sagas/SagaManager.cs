using System.Diagnostics;
using System.Threading;
using Chronos.Core.Orders.NiceHash.Events;
using Chronos.Core.Transactions.Events;
using Chronos.Infrastructure;
using Chronos.Infrastructure.Events;
using Chronos.Infrastructure.Logging;
using Chronos.Infrastructure.Sagas;

namespace Chronos.Core.Sagas
{
    public class SagaManager : SagaManagerBase
    { 
        [DebuggerStepThrough]
        public void When(PurchaseCreated e) => When<PurchaseCreated,TransactionSaga>(e, x => x.PurchaseId);
        [DebuggerStepThrough]
        public void When(CommandScheduled e) => When<CommandScheduled, SchedulerSaga>(e, x => x.ScheduleId);
        [DebuggerStepThrough]
        public void When(TimeoutCompleted e) => When<TimeoutCompleted, SchedulerSaga>(e, x => x.ScheduleId, false);

        [DebuggerStepThrough]
        public void TimeElapsed(TimeoutCompleted e) => When<TimeoutCompleted, NicehashOrderSaga>(e, x => x.ScheduleId,false);
        [DebuggerStepThrough]
        public void When(CashTransferCreated e) => When<CashTransferCreated, TransferSaga>(e, x => x.TransferId);

        [DebuggerStepThrough]
        public void When(JsonRequestCompleted e) =>
            When<JsonRequestCompleted, NicehashOrderSaga>(e, x => x.RequestorId);
        [DebuggerStepThrough]
        public void When(NicehashOrderCreated e) => When<NicehashOrderCreated, NicehashOrderSaga>(e, x => x.OrderId);

        public SagaManager(ISagaRepository repository, IDebugLog debugLog,
                    IEventStoreSubscriptions eventStore) : base(repository, debugLog,eventStore)
        {            
            Register<PurchaseCreated>(When);
            Register<CommandScheduled>(When);
            Register<TimeoutCompleted>(When);
            Register<TimeoutCompleted>(TimeElapsed);
            Register<CashTransferCreated>(When);
            Register<NicehashOrderCreated>(When);
            Register<JsonRequestCompleted>(When);
        }
    }
}