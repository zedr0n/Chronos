using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Chronos.Infrastructure;
using Chronos.Infrastructure.Events;
using Chronos.Infrastructure.Misc;
using Chronos.Infrastructure.Sagas;
using NodaTime;

namespace Chronos.Persistence
{
    public class EventStoreSagaRepository : ISagaRepository
    {
        private readonly IEventBus _eventBus;
        private readonly IEventDb _eventDb;
        private readonly IEventStoreConnection _connection;

        public EventStoreSagaRepository(IEventBus eventBus, IEventDb eventDb, IEventStoreConnection connection)
        {
            _eventBus = eventBus;
            _eventDb = eventDb;
            _connection = connection;
        }

        public void Save<T>(T saga) where T : ISaga
        {
            var events = saga.GetUncommittedEvents().ToList();

            var streamName = StreamExtensions.StreamName<T>(saga.SagaId);
            _connection.AppendToStream(streamName,0,events);

            foreach (dynamic e in saga.GetUndispatchedMessages())
                _eventBus.Publish(e);

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

        public void Replay<T>(Instant date) where T : ISaga
        {
            throw new NotImplementedException();
        }
    }
}