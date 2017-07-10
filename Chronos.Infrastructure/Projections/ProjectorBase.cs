﻿using System;
using System.Linq;
using Chronos.Infrastructure.Events;
using NodaTime.Text;

namespace Chronos.Infrastructure.Projections
{
    public abstract class ProjectorBase<T> : IProjector<T>
        where T : class,IProjection, new()
    {
        private readonly IProjectionRepository _repository;

        protected ProjectorBase(IEventBus eventBus, IProjectionRepository repository)
        {
            _repository = repository;
            this.RegisterAll(eventBus);
        }

        public void UpdateProjection(IEvent e, Action<T> action, Func<T,bool> where)
        {
            var projections = _repository.Find(where);
            if (projections == null)
            {
                var p = new T();
                action(p);
                p.LastEvent = e.EventNumber;
                _repository.Add(p);
                return;
            }

            // Timestamp and event number need to be in sync. Is that always true?
            foreach (var p in projections.Where(p => p.LastEvent < e.EventNumber || e.Replaying))
            {
                action(p);
                p.LastEvent = e.EventNumber;
            }
        }
    }
}