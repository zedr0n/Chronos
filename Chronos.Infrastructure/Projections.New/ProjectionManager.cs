using System;
using System.Diagnostics;

namespace Chronos.Infrastructure.Projections.New
{
    public class ProjectionManager : IProjectionManager
    {
        private readonly IEventStoreConnection _connection;
        private readonly IStateWriter _writer;

        public ProjectionManager(IEventStoreConnection connection, IStateWriter writer)
        {
            _connection = connection;
            _writer = writer;
        }

        [DebuggerStepThrough]
        public IProjectionFrom<T> Create<T>() where T : class, IReadModel, new()
        {
            return new Projection<T>(_connection,_writer);
            //return new ProjectionBase();
        }

    }
}