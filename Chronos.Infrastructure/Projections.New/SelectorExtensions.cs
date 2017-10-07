using System;

namespace Chronos.Infrastructure.Projections.New
{
    public static class SelectorExtensions
    {
        public static IObservable<T> Where<T>(this IObservable<T> observable, Selector<T> selector)
        {
            return selector.Apply(observable);
        }
    }
}