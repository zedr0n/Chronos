using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Chronos.CrossCuttingConcerns.DependencyInjection;
using Chronos.Infrastructure;
using Chronos.Infrastructure.Commands;
using Chronos.Infrastructure.Events;
using Chronos.Infrastructure.Interfaces;
using Chronos.Infrastructure.Logging;
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
        private static int _counter;

        static DTestBase()
        {
            Clock = SystemClock.Instance;
        }
        
        protected DTestBase(ITestOutputHelper output)
        {
            _output = output;
        }

        protected T GetInstance<T>() where T : class
        {
            var container = new Container();
            ICompositionRoot root = new CompositionRoot();
            root.WithDatabase(typeof(T).Name + _counter)
                .InMemory()
                .ComposeApplication(container);

            container.Register<IDebugLog,DebugLogXUnit>(Lifestyle.Singleton);
            container.Verify();
            ((DebugLogXUnit) container.GetInstance<IDebugLog>()).Output = _output;

            _counter++;
            return container.GetInstance<T>();
        }
    }

    public class Bdd
    {
        private readonly IDomainRepository _repository;
        private readonly ICommandBus _commandBus;
        private readonly IEventStoreSubscriptions _eventStore;

        private int _expectedCount = 1;
        private int _actualCount;
        private readonly ReplaySubject<IEvent> _events = new ReplaySubject<IEvent>();
        //protected IObservable<IEvent> Events;
        
        public Bdd(IDomainRepository repository, ICommandBus commandBus, IEventStoreSubscriptions eventStore)
        {
            _repository = repository;
            _commandBus = commandBus;
            _eventStore = eventStore;
        }
        
        public Bdd Given<T>(Guid id,params IEvent[] events)
            where T : class,IAggregate,new()
        {
            _repository.Save<T>(id,events);
            return this;
        }

        public Bdd When<TCommand>(TCommand command)
            where TCommand : class,ICommand
        {
            _eventStore.Events.Subscribe(e => _events.OnNext(e));
            _commandBus.Send(command);
            _events.OnCompleted();
            return this;
        }

        public Bdd Expected(int count)
        {
            _expectedCount = count;
            return this;
        }

        public Bdd Then(params Func<IEvent, bool>[] conditions)
        {
            foreach (var c in conditions)
                _events.Subscribe(e => Assert.True(c(e)));
            _events.Subscribe(e => _actualCount++);
            Assert.True(_actualCount == _expectedCount);
            return this;
        }

        public Bdd Then(params Func<IEnumerable<IEvent>, bool>[] conditions)
        {
            var receivedEvents = new List<IEvent>();
            _events.Subscribe(e => receivedEvents.Add(e),
                () => Assert.True(conditions.All(c => c(receivedEvents))));

            _events.Subscribe(e => _actualCount++, () => Assert.Equal(_expectedCount, _actualCount));
            return this;
        }

        public Bdd Then<T>(T @event) where T : IEvent
        {
            Then(events => events.OfType<T>().Single().Same(@event));
            return this;
        }
    }
}