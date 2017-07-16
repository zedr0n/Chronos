using System;
using Chronos.Infrastructure.Interfaces;

namespace Chronos.Infrastructure.Events
{
    public interface IEventBus
    {
        void Subscribe<TEvent>(Action<TEvent> handler) where TEvent : class,IEvent;
        void Unsubscribe<TEvent>(Action<TEvent> handler) where TEvent : class,IEvent;
        void Publish<TEvent>(TEvent e) where TEvent : class,IEvent;

        void Publish(IEvent e);
        //void Publish(object e);
    }
}