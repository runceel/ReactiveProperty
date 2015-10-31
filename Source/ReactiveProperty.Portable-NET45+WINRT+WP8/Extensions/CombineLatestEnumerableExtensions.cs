using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;

namespace Reactive.Bindings.Extensions
{
    public static class CombineLatestEnumerableExtensions
    {
        /// <summary>
        /// Lastest values of each sequence are all true.
        /// </summary>
        public static IObservable<bool> CombineLatestValuesAreAllTrue(
            this IEnumerable<IObservable<bool>> sources) => 
            sources.CombineLatest(xs => xs.All(x => x));

        
        /// <summary>
        /// Lastest values of each sequence are all false.
        /// </summary>
        public static IObservable<bool> CombineLatestValuesAreAllFalse(
            this IEnumerable<IObservable<bool>> sources) =>
            sources.CombineLatest(xs => xs.All(x => !x));
    }
}