﻿using System;
using Chronos.Infrastructure.Interfaces;

namespace Chronos.Infrastructure.Events
{
    public interface IEventBus
    {
        void Subscribe<TEvent>(Action<TEvent> handler) where TEvent : IEvent;
        void Unsubscribe<TEvent>(Action<TEvent> handler) where TEvent : IEvent;
        void Publish<TEvent>(TEvent e) where TEvent : IEvent;
        //void Publish(object e);
    }
}