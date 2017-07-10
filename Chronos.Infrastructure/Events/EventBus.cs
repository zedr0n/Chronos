using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using Chronos.Infrastructure.Logging;
using NodaTime;
using NodaTime.Text;

namespace Chronos.Infrastructure.Events
{
    public class EventBus : IEventBus
    {
        private readonly Dictionary<Type, List<object>> _subscribers = new Dictionary<Type, List<object>>();
        private readonly IDebugLog _debugLog;

        public EventBus(IDebugLog debugLog)
        {
            _debugLog = debugLog;
        }

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
                _debugLog.WriteLine(e.GetType().Name + (e.Replaying ? "[R]" : "") + "( " + InstantPattern.ExtendedIso.Format(e.Timestamp) + " )");
                foreach (Action<TEvent> handler in handlers.AsReadOnly())
                {
                    _debugLog.WriteLine(" -> " + handler?.Target.GetType().Name);
                    handler?.Invoke(e);
                }
                _debugLog.WriteLine("");
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