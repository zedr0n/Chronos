using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;

namespace Chronos.Infrastructure.Projections
{
    public class Selector<T>
    {
        private class Predicate
        {
            public Predicate(Func<T, bool> selector)
            {
                Selector = selector;
            }

            public enum PredicateType
            {
                And,
                Or
            }
            
            public Func<T,bool> Selector { get; }
            public PredicateType Type { get; set; } = PredicateType.And;

        }
        
        private readonly LinkedList<Predicate> _predicates = new LinkedList<Predicate>();
        
        public Selector<T> Where(Func<T, bool> selector)
        {
            _predicates.AddLast(new Predicate(selector));
            return this;
        }

        public Selector<T> Or(Func<T, bool> selector)
        {
            _predicates.AddLast(new Predicate(selector)
            {
                Type = Predicate.PredicateType.Or
            });
            return this;
        }

        public IObservable<T> Apply(IObservable<T> observable)
        {
            return _predicates.Aggregate(observable, (current, p) => p.Type == Predicate.PredicateType.And ? 
                current.Where(p.Selector) : current.Merge(observable.Where(p.Selector)));
        }
    }
}