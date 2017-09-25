using System;

namespace Chronos.Infrastructure.Projections.New
{
    public static class ObservableExtensions
    {
        public static IObservable<T> Where<T>(this IObservable<T> observable, Selector<T> selector)
        {
            return selector.Apply(observable);
        }
    }
}