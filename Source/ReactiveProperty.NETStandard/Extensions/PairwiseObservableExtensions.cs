using System;
using System.Reactive.Linq;

namespace Reactive.Bindings.Extensions
{
    public static class ObservablePairwiseExtensions
    {
        /// <summary>Projects old and new element of a sequence into a new form.</summary>
        public static IObservable<OldNewPair<T>> Pairwise<T>(this IObservable<T> source) =>
            Pairwise(source, (x, y) => new OldNewPair<T>(x, y));

        /// <summary>Projects old and new element of a sequence into a new form.</summary>
        public static IObservable<TR> Pairwise<T, TR>(this IObservable<T> source, Func<T, T, TR> selector)
        {
            var result = Observable.Create<TR>(observer =>
            {
                T prev = default(T);
                var isFirst = true;

                return source.Subscribe(x =>
                {
                    if (isFirst)
                    {
                        isFirst = false;
                        prev = x;
                        return;
                    }

                    TR value;
                    try
                    {
                        value = selector(prev, x);
                        prev = x;
                    }
                    catch (Exception ex)
                    {
                        observer.OnError(ex);
                        return;
                    }

                    observer.OnNext(value);
                }, observer.OnError, observer.OnCompleted);
            });

            return result;
        }
    }
}