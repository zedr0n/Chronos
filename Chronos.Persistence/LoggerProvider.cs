using System;
using System.IO;
using Microsoft.Extensions.Logging;

namespace Chronos.Persistence
{
    public class LoggerProvider : ILoggerProvider
    {
        private Logger _current; 

        private static readonly string[] Categories =
        {
        };

        public LoggerProvider()
        {
        }

        public ILogger CreateLogger(string categoryName)
        {
            //if (!Categories.Contains(categoryName))
            //    return new NullLogger();
            return _current ?? (_current = new Logger());
        }

        private class Logger : ILogger
        {
            public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
            {
                if(IsEnabled(logLevel))
                    File.AppendAllText(@".\EF.LOG", formatter(state, exception));
                //Console.WriteLine(formatter(state, exception));
            }

            public bool IsEnabled(LogLevel logLevel)
            {
                return true;
            }

            public IDisposable BeginScope<TState>(TState state)
            {
                return null;
            }

            public Logger()
            {
                File.Delete(@".\EF.LOG");
            }
        }

        public void Dispose()
        {
        }
    }
}
