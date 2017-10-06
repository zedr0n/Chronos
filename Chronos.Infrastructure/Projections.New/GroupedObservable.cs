using System;
using System.Reactive.Linq;

namespace Chronos.Infrastructure.Projections.New
{
    public class GroupedObservable<TKey,T>
    {
        public TKey Key { get; set; }
        public IObservable<T> Observable { get; set; }
    }

    public static class GroupExtensions
    {
        public static IObservable<GroupedObservable<TKey,T>> Transform<TKey, T>(
            this IObservable<GroupedObservable<TKey, T>> observable, Func<IObservable<T>, IObservable<T>> transform)
        {
            return observable.Select(x => new GroupedObservable<TKey,T>
            {
                Key = x.Key,
                Observable = transform(x.Observable)
            });
        }
    }
}