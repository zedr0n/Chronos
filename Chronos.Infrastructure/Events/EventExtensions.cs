using System.Linq;
using System.Reflection;
using Chronos.Infrastructure.Interfaces;

namespace Chronos.Infrastructure.Events
{
    public static class EventExtensions
    {
        public static bool Same<T>(this T left, T right ) where T : IEvent
        {
            var baseProperties = typeof(EventBase).GetRuntimeProperties();
            var properties = typeof(T).GetRuntimeProperties()
                .Where(x => baseProperties.All(y => y.Name != x.Name)).ToList();
            return properties.Select(p => p.GetValue(left))
                .SequenceEqual(properties.Select(p => p.GetValue(right)));
        }
    }
}