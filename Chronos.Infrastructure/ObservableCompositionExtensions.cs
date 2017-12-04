using System;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace Chronos.Infrastructure
{
    public static class ObservableCompositionExtensions
    {
        private static void OnCompleted<TResult>(IObserver<TResult> observer,ref bool completed)
        {
            if(completed)
               observer.OnCompleted(); 
        }
        
        public static IObservable<T> DelayBetweenValues<T>(this IObservable<T> observable, TimeSpan interval)
        {
            var offset =  TimeSpan.Zero;
            return observable
                .Take(1)
                .TimeInterval()
                .Delay(ti =>
                {
                    offset = ti.Interval < interval ? offset.Add(interval - ti.Interval) : TimeSpan.Zero;
                    return Observable.Timer(offset);
                })
                .Select(ti => ti.Value);
        }
        
        public static IObservable<T> DelayBetweenValuesOther<T>(this IObservable<T> observable, TimeSpan interval)
        {
            var span = interval;
            return observable.Take(1).Select(x => Observable.Timer(span).Select(l => x))
                .Do(x => span += interval)
                .Concat();


            //return observable.Scan(Observable.Empty<T>(), (obs, cur) =>
            //    obs.Concat(Observable.Timer(interval).Select(l => cur))).SelectMany(x => x);
            //return observable.SelectMany(x => Observable.Timer(interval).Select(l => x).Concat(Observable.Return(x)));
        } 
        
        public static IObservable<T> DelayBetweenSubscriptions<T>(this IObservable<T> observable, TimeSpan interval)
        {
            return observable
                .TimeInterval()
                .Delay(Observable.Interval(interval),x => Observable.Return((long) 0))
                .Select(ti => ti.Value);
        }
        
        public static IObservable<TSource> CompleteOn<TSource>(this IObservable<TSource> o, bool completed)
        {
            return Observable.Create((IObserver<TSource> observer) =>
            {
                var subscription = o.Subscribe(observer.OnNext, observer.OnCompleted);

                var intervalSub = Observable.Interval(TimeSpan.FromMilliseconds(100))
                    .Subscribe(l => OnCompleted(observer,ref completed));
                
                return Disposable.Create(() =>
                {
                    intervalSub.Dispose();
                    subscription.Dispose();
                });
            });
        }
    }
}