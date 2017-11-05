using Chronos.CrossCuttingConcerns.DependencyInjection;
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

        protected virtual ICompositionRoot CreateRoot(string dbName)
        {
            return new CompositionRoot()
                .WriteWith()/*.InMemory()*/.Database(dbName); 
        }

        protected Container CreateContainer(string dbName)
        {
            lock(_lock)
            {
                var container = new Container();
                CreateRoot(dbName).ComposeApplication(container);
                
                container.Register<IDebugLog,DebugLogXUnit>(Lifestyle.Singleton);
                container.Verify();
                ((DebugLogXUnit) container.GetInstance<IDebugLog>()).Output = _output;
                return container;
            }
        }
    }

    public class ReadTestBase : TestBase
    {
        public ReadTestBase(ITestOutputHelper output) : base(output) {}

        protected override ICompositionRoot CreateRoot(string dbName)
        {
            return base.CreateRoot(dbName)
                .ReadWith().Database(dbName + ".Read");
        }
    }
}