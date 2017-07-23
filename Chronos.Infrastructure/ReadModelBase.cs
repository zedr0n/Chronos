using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Chronos.Infrastructure.Interfaces;

namespace Chronos.Infrastructure
{
    public abstract class ReadModelBase : IReadModel
    {
        private readonly Dictionary<Type, Action<IEvent>> _when = new Dictionary<Type, Action<IEvent>>();

        protected ReadModelBase()
        {
            foreach (var m in GetType().GetRuntimeMethods()
                .Where(m => m.Name == "When")
                .Where(m => m.GetParameters().First().ParameterType != typeof(IEvent)))
            {
                _when.Add(m.GetParameters().First().ParameterType, (e) => m.Invoke(this, new object[] { e }));
            }
        }
        public void When(IEvent e)
        {
            if (_when.ContainsKey(e.GetType()))
                _when[e.GetType()](e);
        }
    }

    public abstract class ReadModelBase<TKey> : ReadModelBase, IReadModel<TKey>
    {
        public TKey Key { get; set; }
    }
}