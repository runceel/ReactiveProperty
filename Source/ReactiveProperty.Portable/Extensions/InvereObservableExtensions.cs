using System;
using System.Linq;
using System.Reactive.Linq;

namespace Reactive.Bindings.Extensions
{
    /// <summary>
    /// IObservable of bool extension methods.
    /// </summary>
    public static class InvereObservableExtensions
    {
        /// <summary>
        /// Inverse bool value.
        /// </summary>
        /// <param name="self">IObservable of bool</param>
        /// <returns>Inverse IObservable of bool</returns>
        public static IObservable<bool> Inverse(this IObservable<bool> self)
        {
            return self.Select(x => !x);
        }
    }
}
