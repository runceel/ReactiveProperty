using System;
using System.Reactive.Linq;

namespace Reactive.Bindings.Interactivity
{
    /// <summary>
    /// EventToReactiveCommand's converter.
    /// </summary>
    /// <typeparam name="T">source type</typeparam>
    /// <typeparam name="U">dest type</typeparam>
    public abstract class DelegateConverter<T, U> : ReactiveConverter<T, U>
    {
        protected override IObservable<U> Convert(IObservable<T> source)
        {
            return source.Select(this.Convert);
        }

        /// <summary>
        /// converter method.
        /// </summary>
        /// <param name="source">source value</param>
        /// <returns>dest value</returns>
        protected abstract U Convert(T source);
    }
}
