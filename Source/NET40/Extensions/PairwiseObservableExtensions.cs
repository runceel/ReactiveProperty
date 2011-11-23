using System;
using System.Diagnostics.Contracts;
#if WINDOWS_PHONE
using Microsoft.Phone.Reactive;
#else
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Subjects;
#endif

namespace Codeplex.Reactive.Extensions
{
    public static class ObservablePairwiseExtensions
    {
        /// <summary>Projects old and new element of a sequence into a new form.</summary>
        public static IObservable<OldNewPair<T>> Pairwise<T>(this IObservable<T> source)
        {
            Contract.Requires<ArgumentNullException>(source != null);
            Contract.Ensures(Contract.Result<IObservable<OldNewPair<T>>>() != null);

            return Pairwise(source, (x, y) => new OldNewPair<T>(x, y));
        }

        /// <summary>Projects old and new element of a sequence into a new form.</summary>
        public static IObservable<TR> Pairwise<T, TR>(this IObservable<T> source, Func<T, T, TR> selector)
        {
            Contract.Requires<ArgumentNullException>(source != null);
            Contract.Requires<ArgumentNullException>(selector != null);
            Contract.Ensures(Contract.Result<IObservable<TR>>() != null);

            var result = Observable.Defer<TR>(() =>
            {
                T prev = default(T);
                var isFirst = true;

                return source.Select(x =>
                {
                    if (isFirst)
                    {
                        isFirst = false;
                        prev = x;
                        return default(TR);
                    }
                    else
                    {
                        var value = selector(prev, x);
                        prev = x;
                        return value;
                    }
                }).Skip(1);
            });

            Contract.Assume(result != null);
            return result;
        }
    }
}