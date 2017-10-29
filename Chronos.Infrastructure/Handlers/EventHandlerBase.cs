using System;
using System.Reactive.Linq;
using Chronos.Infrastructure.Commands;
using Chronos.Infrastructure.Interfaces;

namespace Chronos.Infrastructure.Handlers
{
    public abstract class EventHandlerBase<T> : IEventHandler<T> where T : IEvent
    {
        private readonly ICommandBus _commandBus;

        protected EventHandlerBase(ICommandBus commandBus, IEventStore eventStore)
        {
            _commandBus = commandBus;

            var alerts = eventStore.Alerts.Publish();
            alerts.Connect();
            
            alerts.OfType<T>().Subscribe(Handle);
        }

        protected void SendCommand(ICommand command)
        {
            _commandBus.Send(command);
        }

        public abstract void Handle(T @event);
    }
}