using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Logging;

namespace Chronos.Persistence
{
    public static class LoggingExtensions
    {
        public static void LogToConsole(this DbContext context)
        {
            // IServiceProvider represents registered DI container
            var serviceProvider = context.GetInfrastructure();

            // Get the registered ILoggerFactory from the DI container
            var loggerFactory = (ILoggerFactory)serviceProvider.GetService(typeof(ILoggerFactory));

            // Add a logging provider with a console trace listener
            loggerFactory.AddProvider(new LoggerProvider());
            //loggerFactory.AddConsole(LogLevel.Verbose);
        }

        public static void StopLogging(this DbContext context)
        {
            var serviceProvider = context.GetInfrastructure<IServiceProvider>();

            // Get the registered ILoggerFactory from the DI container
            var loggerFactory = (ILoggerFactory)serviceProvider.GetService(typeof(ILoggerFactory));

            loggerFactory.Dispose();
        }
    }

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
