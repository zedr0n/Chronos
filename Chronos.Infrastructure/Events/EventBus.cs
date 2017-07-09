using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

namespace Chronos.Infrastructure.Events
{
    public class EventBus : IEventBus
    {
        private readonly Dictionary<Type, List<object>> _subscribers = new Dictionary<Type, List<object>>();
        public void Subscribe<TEvent>(Action<TEvent> handler) where TEvent : IEvent
        {
            if (_subscribers.ContainsKey(typeof(TEvent)))
            {
                var handlers = _subscribers[typeof(TEvent)];
                if(!handlers.Contains(handler))
                    handlers.Add(handler);
            }
            else
            {
                var handlers = new List<object> { handler };
                _subscribers[typeof(TEvent)] = handlers;
            }
        }

        public void Unsubscribe<TEvent>(Action<TEvent> handler) where TEvent : IEvent
        {
            if (_subscribers.ContainsKey(typeof(TEvent)))
            {
                var handlers = _subscribers[typeof(TEvent)];
                handlers.Remove(handler);

                if (handlers.Count == 0)
                {
                    _subscribers.Remove(typeof(TEvent));
                }
            }
        }

        public void Publish<TEvent>(TEvent e) where TEvent : IEvent
        {
            if (_subscribers.ContainsKey(typeof(TEvent)))
            {
                var handlers = _subscribers[typeof(TEvent)];
                Debug.WriteLine(e.GetType().Name);
                foreach (Action<TEvent> handler in handlers.AsReadOnly())
                {
                    handler?.Invoke(e);
                }
                Debug.WriteLine("");
            }
        }

        /*public void Publish(object message)
        {
            var messageType = message.GetType();
            if (_subscribers.ContainsKey(messageType))
            {
                var handlers = _subscribers[messageType];
                foreach (var handler in handlers)
                {
                    var actionType = handler.GetType();
                    var invoke = actionType.GetRuntimeMethod("Invoke", new Type[] { messageType });
                    invoke.Invoke(handler, new[] { message });
                }
            }
        }*/
    }
}