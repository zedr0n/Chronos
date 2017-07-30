using System.Diagnostics;
using Chronos.Infrastructure.Events;

namespace Chronos.Infrastructure.Projections.New
{
    public class ProjectionManager : IProjectionManager
    {
        private readonly IEventStoreConnection _connection;
        private readonly IStateWriter _writer;
        private readonly IEventBus _eventBus;

        public ProjectionManager(IEventStoreConnection connection, IStateWriter writer, IEventBus eventBus)
        {
            _connection = connection;
            _writer = writer;
            _eventBus = eventBus;
        }

        [DebuggerStepThrough]
        public IProjectionFrom<T> Create<T>() where T : class, IReadModel, new()
        {
            return new Projection<T>(_connection,_writer,_eventBus);
        }
    }
}