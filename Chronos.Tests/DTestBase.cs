using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reflection;
using System.Runtime.CompilerServices;
using Chronos.CrossCuttingConcerns.DependencyInjection;
using Chronos.Infrastructure;
using Chronos.Infrastructure.Commands;
using Chronos.Infrastructure.Events;
using Chronos.Infrastructure.Interfaces;
using Chronos.Infrastructure.Logging;
using Chronos.Infrastructure.Queries;
using NodaTime;
using SimpleInjector;
using Xunit;
using Xunit.Abstractions;

namespace Chronos.Tests
{
    
    public class DTestBase
    {
        protected static readonly IClock Clock;
        private readonly ITestOutputHelper _output;

        static DTestBase()
        {
            Clock = SystemClock.Instance;
        }
        
        protected DTestBase(ITestOutputHelper output)
        {
            _output = output;
        }

        protected T GetInstance<T>([CallerMemberName] string callerName = null) where T : class
        {
            var container = new Container();
            ICompositionRoot root = new CompositionRoot();

            root.WithDatabase(callerName ?? typeof(T).Name)
                .InMemory()
                .ComposeApplication(container);

            container.Register<IDebugLog,DebugLogXUnit>(Lifestyle.Singleton);
            container.Verify();
            ((DebugLogXUnit) container.GetInstance<IDebugLog>()).Output = _output;

            return container.GetInstance<T>();
        }
    }

    public class Specification
    {
        private readonly IDomainRepository _repository;
        private readonly ICommandBus _commandBus;
        private readonly IEventStoreSubscriptions _eventStore;
        private readonly IQueryProcessor _queryProcessor;

        private int _expectedCount = 1;
        private int _actualCount;
        private readonly ReplaySubject<IEvent> _events = new ReplaySubject<IEvent>();
        //protected IObservable<IEvent> Events;
        
        public Specification(IDomainRepository repository,
            ICommandBus commandBus, 
            IEventStoreSubscriptions eventStore,
            IQueryProcessor queryProcessor)
        {
            _repository = repository;
            _commandBus = commandBus;
            _eventStore = eventStore;
            _queryProcessor = queryProcessor;
        }
        
        public Specification Given<T>(Guid id,params IEvent[] events)
            where T : class,IAggregate,new()
        {
            _repository.Save<T>(id,events);
            return this;
        }

        public Specification When<TCommand>(TCommand command)
            where TCommand : class,ICommand
        {
            _eventStore.Events.Subscribe(e => _events.OnNext(e));
            _commandBus.Send(command);
            _events.OnCompleted();
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
            return this;
        }

        public Specification Then<T>(T @event) where T : IEvent
        {
            Then(events => events.OfType<T>().Single().Same(@event));
            return this;
        }

        public Specification Then(params IEvent[] events)
        {
            Then(e => e.All(x => events.Any(x.Same)));
            return Expected(events.Length);
        }
    }
}