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

            return source.Scan(
                    new OldNewPair<T>(default(T), default(T)),
                    (pair, newValue) => new OldNewPair<T>(pair.NewItem, newValue))
                .Skip(1);
        }

        /// <summary>Projects old and new element of a sequence into a new form.</summary>
        public static IObservable<TR> Pairwise<T, TR>(this IObservable<T> source, Func<T, T, TR> selector)
        {
            Contract.Requires<ArgumentNullException>(source != null);
            Contract.Requires<ArgumentNullException>(selector != null);
            Contract.Ensures(Contract.Result<IObservable<TR>>() != null);

            return source.Pairwise().Select(x => selector(x.OldItem, x.NewItem));
        }
    }
}