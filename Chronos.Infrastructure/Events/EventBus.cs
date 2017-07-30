using System;
using System.Collections.Generic;
using System.Linq;
using Chronos.Infrastructure.Interfaces;
using Chronos.Infrastructure.Logging;
using NodaTime.Text;

namespace Chronos.Infrastructure.Events
{
    public class EventBus : IEventBus
    {
        private class Handler
        {
            private Type _target;
            private string FullName => _target.FullName;
            internal string Name => _target.Name;
            internal Action<IEvent> Action { get; private set; }

            internal static Handler Create<T>(Action<T> action) where T : class, IEvent
            {
                return new Handler
                {
                    _target = action.Target.GetType(),
                    Action = e => action(e as T)
                };
            }

            internal bool Is<T>(Action<T> handler) where T : class, IEvent
            {
                return FullName == handler.Target.GetType().FullName;
            }

        }

        private readonly Dictionary<Type, List<Handler>> _subscribers = new Dictionary<Type, List<Handler>>();
        private readonly IDebugLog _debugLog;

        public EventBus(IDebugLog debugLog)
        {
            _debugLog = debugLog;
        }

        public void Subscribe<TEvent>(Action<TEvent> handler) where TEvent : class,IEvent
        {
            if (_subscribers.ContainsKey(typeof(TEvent)))
            {
                var handlers = _subscribers[typeof(TEvent)];
                if (handlers.Any(x => x.Is(handler)))
                    return;

                handlers.Add(Handler.Create(handler));
            }
            else
            {
                var handlers = new List<Handler> { Handler.Create(handler) };
                _subscribers[typeof(TEvent)] = handlers;
            }
        }

        public void Unsubscribe<TEvent>(Action<TEvent> handler) where TEvent : class,IEvent
        {
            if (_subscribers.ContainsKey(typeof(TEvent)))
            {
                var handlers = _subscribers[typeof(TEvent)];
                handlers.Remove(handlers.SingleOrDefault(x => x.Is(handler)));

                if (handlers.Count == 0)
                {
                    _subscribers.Remove(typeof(TEvent));
                }
            }
        }

        public void Publish<TEvent>(TEvent e) where TEvent : class,IEvent
        {
            if (_subscribers.ContainsKey(typeof(TEvent)))
            {
                var handlers = _subscribers[typeof(TEvent)];
                _debugLog.WriteLine(e.GetType().Name + "( " + InstantPattern.ExtendedIso.Format(e.Timestamp) + " )");
                foreach (var handler in handlers)
                {
                    _debugLog.WriteLine(" -> " + handler?.Name);
                    handler?.Action?.Invoke(e);
                }
                _debugLog.WriteLine("");
            }
        }

        public void Publish(IEvent e)
        {
            if (e == null)
                return;
            var type = e.GetType();
            if (_subscribers.ContainsKey(type))
            {
                var handlers = _subscribers[type];
                _debugLog.WriteLine(e.GetType().Name  + "( " +
                                    InstantPattern.ExtendedIso.Format(e.Timestamp) + " )");
                foreach (var handler in handlers)
                {
                    _debugLog.WriteLine(" -> " + handler?.Name);
                    handler?.Action?.Invoke(e);
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