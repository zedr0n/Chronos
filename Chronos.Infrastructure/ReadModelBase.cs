﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using Chronos.Infrastructure.Interfaces;
using NodaTime;

namespace Chronos.Infrastructure
{
    public abstract class ReadModelBase<TKey> : IReadModel<TKey>
    {
        private readonly Dictionary<Type, Action<IEvent>> _when = new Dictionary<Type, Action<IEvent>>();

        [Key]
        public TKey Key { get; set; }
        /// <summary>
        /// Originating stream version
        /// </summary>
        public int Version { get; set; }
        public Guid Timeline { get; set; }

        public DateTime TimestampUtc { get; set; }

        protected ReadModelBase()
        {
            foreach (var m in GetType().GetTypeInfo().GetDeclaredMethods("When"))
                _when.Add(m.GetParameters().First().ParameterType, e => m.Invoke(this, new object[] { e }));
        }
        public virtual bool When(IEvent e)
        {
            if (!_when.ContainsKey(e.GetType()))
                return false;
            
            Version = e.Version;
            
            if(e.Timestamp != Instant.MinValue)
                TimestampUtc = e.Timestamp.ToDateTimeUtc();
            
            _when[e.GetType()](e);
            return true;
        }
    }
}