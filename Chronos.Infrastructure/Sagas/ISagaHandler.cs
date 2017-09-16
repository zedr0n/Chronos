using Chronos.Infrastructure.Interfaces;

namespace Chronos.Infrastructure.Sagas
{
    public interface ISagaHandler
    {
    }
    
    public interface ISagaHandler<T> : ISagaHandler where T : ISaga
    {
    }
}