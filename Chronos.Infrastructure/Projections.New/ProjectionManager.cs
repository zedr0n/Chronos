using System.Diagnostics;
using Chronos.Infrastructure.Events;

namespace Chronos.Infrastructure.Projections.New
{
    public class ProjectionManager : IProjectionManager
    {
        private readonly IEventStoreSubscriptions _eventStore;
        private readonly IStateWriter _writer;
        private readonly IEventBus _eventBus;

        public ProjectionManager(IEventStoreSubscriptions eventStore, IStateWriter writer, IEventBus eventBus)
        {
            _eventStore = eventStore;
            _writer = writer;
            _eventBus = eventBus;
        }


        [DebuggerStepThrough]
        public IProjectionFrom<T> Create<T>() where T : class, IReadModel, new()
        {
            return new Projection<T>(_eventStore,_writer,_eventBus);
        }
    }

}