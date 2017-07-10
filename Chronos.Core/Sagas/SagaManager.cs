using System;
using Chronos.Core.Transactions.Events;
using Chronos.Infrastructure;
using Chronos.Infrastructure.Events;
using Chronos.Infrastructure.Sagas;

namespace Chronos.Core.Sagas
{
    public class SagaManager : ISagaManager,
        IConsumer<PurchaseCreated>,
        IConsumer<CommandScheduled>,
        IConsumer<TimeoutCompleted>
    {
        private readonly ISagaRepository _repository;

        public SagaManager(IEventBus eventBus, ISagaRepository repository)
        {
            _repository = repository;
            this.RegisterAll(eventBus);
        }
        public void When(PurchaseCreated e)
        {
            var sagaId = e.SourceId;
            var saga = _repository.Find<TransactionSaga>(sagaId) ?? new TransactionSaga(sagaId);

            saga.Dispatch(e);

            _repository.Save(saga);
        }

        public void When(CommandScheduled e)
        {
            var sagaId = e.ScheduleId;
            var saga = _repository.Find<SchedulerSaga>(sagaId) ?? new SchedulerSaga(sagaId);

            saga.Dispatch(e);

            _repository.Save(saga);
        }


        public void When(TimeoutCompleted e)
        {
            var sagaId = e.SourceId;
            var saga = _repository.Find<SchedulerSaga>(sagaId) ?? new SchedulerSaga(sagaId);

            saga.Dispatch(e);

            _repository.Save(saga);
        }
    }
}