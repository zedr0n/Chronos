using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Chronos.Infrastructure.Interfaces;

namespace Chronos.Infrastructure.Projections.New
{
    public class ProjectionHandlerBase<T> : IProjectionHandler<T> where T : class, IReadModel, new()
    {
        private readonly Dictionary<Type, Action<T, IEvent>> _when = new Dictionary<Type, Action<T, IEvent>>();

        protected ProjectionHandlerBase()
        {
            _when.Clear();

            foreach (var m in GetType().GetRuntimeMethods()
                .Where(m => m.Name == "When")
                .Where(m => m.GetParameters().Length == 2)
                .Where(m => m.GetParameters().First().ParameterType != typeof(IEvent)))
            {
                _when.Add(m.GetParameters().First().ParameterType, (s, e) => m.Invoke(this, new object[] { s, e }));
            }
        }
        public void When(T s, IEvent e)
        {
            if (_when.ContainsKey(e.GetType()))
                _when[e.GetType()](s, e);
        }
    }
}