using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Chronos.Infrastructure;
using Chronos.Infrastructure.Commands;
using Chronos.Infrastructure.Events;
using Chronos.Infrastructure.Interfaces;
using Chronos.Infrastructure.Queries;
using Xunit;

namespace Chronos.Tests
{
    public class Specification
    {
        private readonly IDomainRepository _repository;
        private readonly ICommandBus _commandBus;
        private readonly IEventStoreSubscriptions _eventStore;
        private readonly IQueryProcessor _queryProcessor;

        private int _expectedCount = 1;
        private int _actualCount;
        private readonly ReplaySubject<IEvent> _events = new ReplaySubject<IEvent>();

        private bool _recording = true;

        private void AddEvent(IEvent e)
        {
            if(_recording)
                _events.OnNext(e);
        }
        
        public Specification(IDomainRepository repository,
            ICommandBus commandBus, 
            IEventStoreSubscriptions eventStore,
            IQueryProcessor queryProcessor)
        {
            _repository = repository;
            _commandBus = commandBus;
            _eventStore = eventStore;
            _queryProcessor = queryProcessor;
            _eventStore.Events.Subscribe(AddEvent);
        }
        
        public Specification Given<T>(Guid id,params IEvent[] events)
            where T : class,IAggregate,new()
        {
            _recording = false;
            _repository.Save<T>(id,events);
            _recording = true;
            return this;
        }

        public Specification When<TCommand>(TCommand command)
            where TCommand : class,ICommand
        {
            _commandBus.Send(command);

            return this;
        }

        public Specification Expected(int count)
        {
            _expectedCount = count;
            return this;
        }

        public TResult Query<TQuery,TResult>(TQuery query) 
            where TQuery : IQuery<TResult> 
            where TResult : class, IReadModel, new()
        {
            return _queryProcessor.Process<TQuery, TResult>(query);
        }

        public Specification Then(params Func<IEvent, bool>[] conditions)
        {
            foreach (var c in conditions)
                _events.Subscribe(e => Assert.True(c(e)));
            _events.Subscribe(e => _actualCount++);
            Assert.True(_actualCount == _expectedCount);
            return this;
        }

        public Specification Then(params Func<IEnumerable<IEvent>, bool>[] conditions)
        {
            var receivedEvents = new List<IEvent>();
            _events.Subscribe(e => receivedEvents.Add(e),
                () => Assert.True(conditions.All(c => c(receivedEvents))));

            _events.Subscribe(e => _actualCount++, () => Assert.Equal(_expectedCount, _actualCount));
            _events.OnCompleted();
            return this;
        }

        public Specification Then<T>(T @event) where T : IEvent
        {
            Then(events => events.OfType<T>().Single().Same(@event));
            return this;
        }

        public Specification Then(params IEvent[] events)
        {
            Expected(events.Length);
            Then(e => e.All(x => events.Any(x.Same)));
            return this;
        }
    }
}