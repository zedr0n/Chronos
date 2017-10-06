using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using Chronos.Infrastructure;
using Chronos.Infrastructure.Commands;
using Chronos.Infrastructure.Interfaces;
using Chronos.Infrastructure.Misc;
using Chronos.Infrastructure.Sagas;
using NodaTime;
using StreamExtensions = Chronos.Infrastructure.StreamExtensions;

namespace Chronos.Persistence
{
    public class EventStoreSagaRepository : ISagaRepository
    {
        private readonly ICommandBus _commandBus;
        private readonly IEventStoreConnection _connection;
        private readonly StreamTracker _streamTracker;

        public EventStoreSagaRepository(IEventStoreConnection connection, ICommandBus commandBus, StreamTracker streamTracker)
        {
            _connection = connection;
            _commandBus = commandBus;
            _streamTracker = streamTracker;
        }

        public void Save<T>(T saga) where T : class,ISaga, new()
        {
            var events = saga.UncommitedEvents.ToList();

            var expectedVersion = saga.Version - events.Count;

            StreamDetails stream;
            if (expectedVersion == 0)
                stream = _streamTracker.Add(saga);
            else 
                stream = _streamTracker.Get(saga);  
            
            _connection.AppendToStream(stream,expectedVersion,events);

            foreach (var e in saga.UndispatchedMessages)
                _commandBus.Send(e as ICommand);

            saga.ClearUncommittedEvents();
            saga.ClearUndispatchedMessages();
        }

        public T Find<T>(Guid id) where T : class,ISaga, new()
        {         
            //var stream = new StreamDetails(typeof(T),id);
            var stream = _streamTracker.Get(typeof(T), id);
            if (stream == null)
                return null;
            
            var events = _connection.ReadStreamEventsForward(stream, -1, int.MaxValue).ToList();

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