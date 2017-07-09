using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using Chronos.Infrastructure;
using Chronos.Infrastructure.Commands;
using Chronos.Infrastructure.Events;
using Chronos.Infrastructure.Misc;
using Chronos.Infrastructure.Sagas;
using NodaTime;

namespace Chronos.Persistence
{
    public class EventStoreSagaRepository : ISagaRepository
    {
        private readonly IEventBus _eventBus;
        private readonly ICommandBus _commandBus;
        private readonly IEventStoreConnection _connection;

        public EventStoreSagaRepository(IEventBus eventBus, IEventStoreConnection connection, ICommandBus commandBus)
        {
            _eventBus = eventBus;
            _connection = connection;
            _commandBus = commandBus;
        }

        public void Save<T>(T saga) where T : ISaga
        {
            var events = saga.UncommitedEvents.ToList();

            var streamName = StreamExtensions.StreamName<T>(saga.SagaId);
            var expectedVersion = saga.Version - events.Count;
            _connection.AppendToStream(streamName,expectedVersion,events);

            foreach (var e in saga.UndispatchedMessages)
            {
                if(typeof(ICommand).GetTypeInfo().IsAssignableFrom(e.GetType().GetTypeInfo()))
                    _commandBus.Send((dynamic) e);
                else
                    _eventBus.Publish((dynamic) e);
            }

            saga.ClearUncommittedEvents();
            saga.ClearUndispatchedMessages();
        }

        public T Find<T>(Guid id) where T : ISaga
        {
            var streamName = StreamExtensions.StreamName<T>(id);
            var events = _connection.ReadStreamEventsForward(streamName, 0, int.MaxValue).AsCachedAnyEnumerable();

            if (events.Any())
            {
                var saga = (T) Activator.CreateInstance(typeof(T), id, events);
                saga.ClearUncommittedEvents();
                saga.ClearUndispatchedMessages();
                return saga;
            }

            return default(T);
        }

        public T Get<T>(Guid id) where T : ISaga
        {
            var saga = Find<T>(id);
            if(saga == null)
                throw new InvalidOperationException("No saga with such id exists");
            return saga;
        }
    }
}