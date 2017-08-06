using System;
using System.Collections.Generic;
using System.Linq;
using Chronos.CrossCuttingConcerns.DependencyInjection;
using NodaTime;
using SimpleInjector;
using Xunit.Abstractions;
using Chronos.Infrastructure.Logging;
using Xunit;
using Xunit.Sdk;

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
                ICompositionRoot root = new CompositionRoot();
                root.WithDatabase(dbName)
                    .InMemory()
                    .ComposeApplication(container);

                container.Register<IDebugLog,DebugLogXUnit>(Lifestyle.Singleton);
                container.Verify();
                ((DebugLogXUnit) container.GetInstance<IDebugLog>()).Output = _output;
                return container;
            }
        }
    }
}