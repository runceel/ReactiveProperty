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
        /// <summary>
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        protected override IObservable<U> OnConvert(IObservable<T> source) => source.Select(OnConvert);

        /// <summary>
        /// converter method.
        /// </summary>
        /// <param name="source">source value</param>
        /// <returns>dest value</returns>
        protected abstract U OnConvert(T source);
    }
}
