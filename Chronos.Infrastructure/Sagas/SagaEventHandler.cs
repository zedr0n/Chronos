using System;
using Chronos.Infrastructure.Interfaces;
using Chronos.Infrastructure.Logging;

namespace Chronos.Infrastructure.Sagas
{
    public class SagaEventHandler : ISagaEventHandler
    {
        private readonly IDebugLog _debugLog;

        public SagaEventHandler(IDebugLog debugLog)
        {
            _debugLog = debugLog;
        }

        public void Handler<TEvent, T>(T saga, TEvent e) where TEvent : class, IEvent where T : class, ISaga
        {
            try
            {
                //(saga as IHandle<TEvent>)?.When(e);
                saga.When(e);
            }
            catch (Exception exception)
            {
                _debugLog.WriteLine(exception.Message);
                throw;
            }
        }
    }
}