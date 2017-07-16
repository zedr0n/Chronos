using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Chronos.Infrastructure.Interfaces;

namespace Chronos.Infrastructure.Events
{
    public static class ConsumerExtensions
    {
        [DebuggerStepThrough]
        public static IEnumerable<Type> Interfaces(this IConsumer consumer)
        {
            var interfaces = consumer.GetType().GetTypeInfo().ImplementedInterfaces;
            interfaces = interfaces
                //.Where(t => typeof(IConsumer).GetTypeInfo().IsAssignableFrom(t.GetTypeInfo()))
                .Where(t => t.GetTypeInfo().ImplementedInterfaces.FirstOrDefault() == typeof(IConsumer));
            interfaces = interfaces
                .Where(t => t.GenericTypeArguments.Length > 0);
            return interfaces;
        }

        public static void RegisterAll(this IConsumer consumer,IEventBus eventBus)
        {
            foreach (var e in consumer.Interfaces().Select(i => i.GenericTypeArguments.Single()))
                consumer.Register(e,eventBus);
        }

        [DebuggerStepThrough]
        public static void Register(this IConsumer consumer, Type eventType, IEventBus eventBus)
        {
            typeof(ConsumerExtensions).GetTypeInfo().GetDeclaredMethods(nameof(Register)).ToList()
                .Single(m => m.IsGenericMethod)
                .MakeGenericMethod(eventType)
                .Invoke(null, new object[] { consumer, eventBus });
        }

        public static void Register<TEvent>(this IConsumer<TEvent> consumer, IEventBus eventBus)
            where TEvent : class,IEvent
        {
            eventBus.Subscribe<TEvent>(consumer.When);
        }

        [DebuggerStepThrough]
        public static bool Dispatch(this IConsumer consumer, IEvent e)
        {
            var interfaces = consumer.Interfaces();
            if (interfaces.Select(i => i.GenericTypeArguments.Single()).All(t => t != e.GetType()))
                return false;

            typeof(ConsumerExtensions).GetTypeInfo().GetDeclaredMethods(nameof(Dispatch)).ToList()
                .Single(m => m.IsGenericMethod)
                .MakeGenericMethod(e.GetType())
                .Invoke(null, new object[] { consumer, e });

            return true;
        }
        [DebuggerStepThrough]
        public static void Dispatch<TEvent>(this IConsumer<TEvent> consumer, TEvent e) where TEvent : IEvent
        {
            consumer.When(e);
        }

    }
}