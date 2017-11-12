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