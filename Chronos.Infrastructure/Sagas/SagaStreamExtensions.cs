namespace Chronos.Infrastructure.Sagas
{
    public static class SagaStreamExtensions
    {
        public static StreamDetails StreamDetails(this ISaga saga)
        {
            return new StreamDetails(name: $"{saga.GetType().Name}-{saga.SagaId}", sourceType: saga.GetType());
        }
    }
}
