using Chronos.Core.Transactions.Events;
using Chronos.Infrastructure;
using Chronos.Infrastructure.Events;
using Chronos.Infrastructure.Sagas;

namespace Chronos.Core.Sagas
{
    public class SagaManager : ISagaManager,
        IConsumer<PurchaseCreated>
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
    }
}