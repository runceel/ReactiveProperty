using System;
using System.Reactive.Linq;
using System.Windows.Threading;
using GalaSoft.MvvmLight.Threading;

namespace Codeplex.Reactive
{
    public static class UIDispatcherObservableExtensions
    {
        public static IObservable<T> ObserveOnUIUIDispatcher<T>(this IObservable<T> source)
        {
            return source.ObserveOn(new DispatcherSynchronizationContext(DispatcherHelper.UIDispatcher));
        }

        public static IObservable<T> SubscribeOnUIDispatcher<T>(this IObservable<T> source)
        {
            return source.SubscribeOn(new DispatcherSynchronizationContext(DispatcherHelper.UIDispatcher));
        }
    }
}