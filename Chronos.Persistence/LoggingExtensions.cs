using System;
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
            var serviceProvider = context.GetInfrastructure();

            // Get the registered ILoggerFactory from the DI container
            var loggerFactory = (ILoggerFactory)serviceProvider.GetService(typeof(ILoggerFactory));

            loggerFactory.Dispose();
        }
    }
}