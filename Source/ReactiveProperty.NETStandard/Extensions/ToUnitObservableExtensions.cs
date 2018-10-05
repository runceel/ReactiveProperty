using System;
using System.Reactive;
using System.Reactive.Linq;

namespace Reactive.Bindings.Extensions
{
    /// <summary>
    /// To Unit Observable Extensions
    /// </summary>
    public static class ToUnitObservableExtensions
    {
        /// <summary>
        /// To the unit.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="self">The self.</param>
        /// <returns></returns>
        public static IObservable<Unit> ToUnit<T>(this IObservable<T> self) =>
            self.Select(_ => Unit.Default);
    }
}
