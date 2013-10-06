using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
#if WINDOWS_PHONE
using Microsoft.Phone.Reactive;
#else
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
#endif

namespace Codeplex.Reactive.Extensions
{
    public static class CombineLatestEnumerableExtensions
    {
        /// <summary>
        /// Lastest values of each sequence are all true.
        /// </summary>
        public static IObservable<bool> CombineLatestValuesAreAllTrue(
            this IEnumerable<IObservable<bool>> sources)
        {
            var result = sources.Aggregate((xs, ys) =>
                xs.CombineLatest(ys, (x, y) => x && y));
            return result;
        }
    }
}