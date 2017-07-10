using System;
using System.Diagnostics;
using System.IO;
using Chronos.CrossCuttingConcerns.DependencyInjection;
using Chronos.Infrastructure;
using NodaTime;
using SimpleInjector;
using Xunit.Abstractions;
using Chronos.Infrastructure.Logging;

namespace Chronos.Tests
{
    public class TestsBase
    {
        protected static readonly IClock Clock;
        private readonly ITestOutputHelper _output;
        private readonly object _lock = new object();

        static TestsBase()
        {
            Clock = SystemClock.Instance;
        }

        public TestsBase(ITestOutputHelper output)
        {
            _output = output;
        }

        protected Container CreateContainer(string dbName)
        {
            lock(_lock)
            {
                var container = new Container();
                new CompositionRoot().ComposeApplication(container, dbName, false, true);
                container.Register<IDebugLog,DebugLogXUnit>(Lifestyle.Singleton);
                container.Verify();
                container.GetInstance<IEventStoreConnection>().Initialise();
                ((DebugLogXUnit) container.GetInstance<IDebugLog>()).Output = _output;
                return container;
            }
        }
    }
}