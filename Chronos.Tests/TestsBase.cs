using Chronos.CrossCuttingConcerns.DependencyInjection;
using Chronos.Infrastructure;
using NodaTime;
using SimpleInjector;

namespace Chronos.Tests
{
    public class TestsBase
    {
        protected static readonly IClock Clock;
        private readonly object _lock = new object();

        static TestsBase()
        {
            Clock = SystemClock.Instance;
        }

        protected Container CreateContainer(string dbName)
        {
            lock(_lock)
            {
                var container = new Container();
                new CompositionRoot().ComposeApplication(container, dbName, false, true);
                container.Verify();
                container.GetInstance<IEventStoreConnection>().Initialise();
                return container;
            }
        }
    }
}