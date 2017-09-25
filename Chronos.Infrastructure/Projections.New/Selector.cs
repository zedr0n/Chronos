using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;

namespace Chronos.Infrastructure.Projections.New
{
    public class Selector<T>
    {
        private readonly LinkedList<Func<T, bool>> _predicates = new LinkedList<Func<T, bool>>();
        
        public Selector<T> Where(Func<T, bool> selector)
        {
            _predicates.AddLast(selector);
            return this;
        }

        public IObservable<T> Apply(IObservable<T> observable)
        {
            return _predicates.Aggregate(observable, (current, p) => current.Where(p));
        }
    }
}