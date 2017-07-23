using Chronos.CrossCuttingConcerns.DependencyInjection;
using Chronos.Infrastructure;
using NodaTime;
using SimpleInjector;
using Xunit.Abstractions;
using Chronos.Infrastructure.Logging;

namespace Chronos.Tests
{
    public class TestBase
    {
        protected static readonly IClock Clock;
        private readonly ITestOutputHelper _output;
        private readonly object _lock = new object();

        static TestBase()
        {
            Clock = SystemClock.Instance;
        }

        protected TestBase(ITestOutputHelper output)
        {
            _output = output;
        }

        protected Container CreateContainer(string dbName)
        {
            lock(_lock)
            {
                var container = new Container();
                var root = new CompositionRoot().WithDatabase(dbName).InMemory();
                root.ComposeApplication(container);

                container.Register<IDebugLog,DebugLogXUnit>(Lifestyle.Singleton);
                container.Verify();
                container.GetInstance<IEventStoreConnection>().Initialise();
                ((DebugLogXUnit) container.GetInstance<IDebugLog>()).Output = _output;
                return container;
            }
        }
    }
}