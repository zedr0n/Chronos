using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Chronos.Infrastructure.Interfaces;

namespace Chronos.Infrastructure
{
    public abstract class ReadModelBase<TKey> : IReadModel<TKey>
    {
        private readonly Dictionary<Type, Action<IEvent>> _when = new Dictionary<Type, Action<IEvent>>();

        public TKey Key { get; set; }
        
        protected ReadModelBase()
        {
            foreach (var m in GetType().GetTypeInfo().GetDeclaredMethods("When"))
                _when.Add(m.GetParameters().First().ParameterType, (e) => m.Invoke(this, new object[] { e }));
        }
        public void When(IEvent e)
        {
            if (_when.ContainsKey(e.GetType()))
                _when[e.GetType()](e);
        }
    }
}