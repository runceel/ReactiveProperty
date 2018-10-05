using System;
using System.Reactive.Linq;
using System.Windows;

#if NETFX_CORE
using Windows.UI.Xaml;
#endif

namespace Reactive.Bindings.Interactivity
{
    /// <summary>
    /// EventToReactiveCommand's converter.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="U"></typeparam>
    public abstract class ReactiveConverter<T, U> : IEventToReactiveConverter
    {
        /// <summary>
        /// EventToReactiveCommand's AssociateObject
        /// </summary>
        public object AssociateObject { get; set; }

        /// <summary>
        /// Converts the specified source.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns></returns>
        public IObservable<object> Convert(IObservable<object> source) => OnConvert(source.Cast<T>()).Select(x => (object)x);

        /// <summary>
        /// Converts
        /// </summary>
        /// <param name="source">source</param>
        /// <returns>dest</returns>
        protected abstract IObservable<U> OnConvert(IObservable<T> source);
    }
}
