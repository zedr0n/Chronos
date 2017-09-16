using System.Diagnostics;
using System.IO.Compression;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using Chronos.Core.Orders.NiceHash;
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
        //[DebuggerStepThrough]
        //public void When(CommandScheduled e) => When<CommandScheduled, SchedulerSaga>(e, x => x.ScheduleId);
        //[DebuggerStepThrough]
        //public void When(TimeoutCompleted e) => When<TimeoutCompleted, SchedulerSaga>(e, x => x.ScheduleId, false);

        [DebuggerStepThrough]
        public void When(CashTransferCreated e) => When<CashTransferCreated, TransferSaga>(e, x => x.TransferId);

        public SagaManager(ISagaRepository repository, IDebugLog debugLog,
                    IEventStoreSubscriptions eventStore) : base(repository, debugLog,eventStore)
        {
            Register<PurchaseCreated>(When);
            //Register<CommandScheduled>(When);
            //Register<TimeoutCompleted>(When);
            Register<CashTransferCreated>(When);
        }
    }
}