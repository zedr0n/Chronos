﻿using Chronos.CrossCuttingConcerns.DependencyInjection;
using NodaTime;
using SimpleInjector;

namespace Chronos.Tests
{
    public class TestsBase
    {
        protected static readonly IClock Clock;

        static TestsBase()
        {
            Clock = SystemClock.Instance;
        }

        protected static Container CreateContainer(string dbName)
        {
            var container = new Container();
            new CompositionRoot().ComposeApplication(container, dbName, false, true);
            container.Verify();
            return container;
        }
    }
}