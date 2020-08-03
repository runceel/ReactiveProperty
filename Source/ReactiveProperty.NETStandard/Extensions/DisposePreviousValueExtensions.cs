using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace Reactive.Bindings.Extensions
{
    /// <summary>
    /// DisposePreviousValue Ovservable Extensions
    /// </summary>
    public static class DisposePreviousValueExtensions
    {
        /// <summary>
        /// Dispose previous value automatically.
        /// </summary>
        public static IObservable<T> DisposePreviousValue<T>(this IObservable<T> source) => 
            Observable.Create<T>(ox =>
            {
                var d = new SerialDisposable();
                return new CompositeDisposable(
                    source.Do(x => d.Disposable = x as IDisposable).Do(ox).Subscribe(), 
                    d);
            });
    }
}
