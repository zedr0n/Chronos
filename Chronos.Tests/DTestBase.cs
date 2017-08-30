using System.Collections;
using System.Reactive.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Chronos.CrossCuttingConcerns.DependencyInjection;
using Chronos.Infrastructure.Logging;
using NodaTime;
using SimpleInjector;
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

        protected virtual T GetInstance<T>([CallerMemberName] string callerName = null) where T : class
        {
            var container = new Container();
            
            var root = new CompositionRoot()
                .WriteWith().InMemory().Database("BDD"+ ( callerName ?? typeof(T).Name));
            
            root.ComposeApplication(container);
            container.Register<IDebugLog,DebugLogXUnit>(Lifestyle.Singleton);
            container.Verify();
            ((DebugLogXUnit) container.GetInstance<IDebugLog>()).Output = _output;

            return container.GetInstance<T>();
        }
    }
}