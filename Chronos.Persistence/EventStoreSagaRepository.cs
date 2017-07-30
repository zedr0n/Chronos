using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using Chronos.Infrastructure;
using Chronos.Infrastructure.Commands;
using Chronos.Infrastructure.Events;
using Chronos.Infrastructure.Interfaces;
using Chronos.Infrastructure.Misc;
using Chronos.Infrastructure.Sagas;
using NodaTime;
using StreamExtensions = Chronos.Infrastructure.StreamExtensions;

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

        public void Save<T>(T saga) where T : class,ISaga, new()
        {
            var events = saga.UncommitedEvents.ToList();

            var stream = new StreamDetails(saga);
            _connection.Writer.AppendToStream(stream,saga.Version - events.Count,events);

            foreach (var e in saga.UndispatchedMessages)
            {
                _commandBus.Send(e as ICommand);
                _eventBus.Publish(e as IEvent);
            }

            saga.ClearUncommittedEvents();
            saga.ClearUndispatchedMessages();
        }

        public T Find<T>(Guid id) where T : class,ISaga, new()
        {         
            var stream = new StreamDetails(typeof(T),id);
            var events = _connection.Reader.ReadStreamEventsForward(stream.Name, 0, int.MaxValue).ToList();

            if (!events.Any())
                return null;

            var saga = new T().LoadFrom<T>(id,events);

            saga.ClearUncommittedEvents();
            saga.ClearUndispatchedMessages();
            return saga;
        }

        public T Get<T>(Guid id) where T : class,ISaga,new()
        {
            var saga = Find<T>(id);
            if(saga == null)
                throw new InvalidOperationException("No saga with such id exists");
            return saga;
        }
    }
}