using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    public static class UIDispatcherObservableExtensions
    {
        public static IObservable<T> ObserveOnUIDispatcher<T>(this IObservable<T> source)
        {
            Contract.Requires<ArgumentNullException>(source != null);
            Contract.Ensures(Contract.Result<IObservable<T>>() != null);

            var result = source.ObserveOn(UIDispatcherScheduler.Default);

            Contract.Assume(result != null);
            return result;
        }

        public static IObservable<T> SubscribeOnUIDispatcher<T>(this IObservable<T> source)
        {
            Contract.Requires<ArgumentNullException>(source != null);
            Contract.Ensures(Contract.Result<IObservable<T>>() != null);

            var result = source.SubscribeOn(UIDispatcherScheduler.Default);

            Contract.Assume(result != null);
            return result;
        }
    }
}