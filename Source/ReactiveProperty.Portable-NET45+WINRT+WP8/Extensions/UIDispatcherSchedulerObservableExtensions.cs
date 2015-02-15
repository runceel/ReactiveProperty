using System;
using System.Reactive.Linq;

namespace Reactive.Bindings.Extensions
{
    public static class UIDispatcherObservableExtensions
    {
        /// <summary>
        /// <para>Observe on UIDispatcherScheduler.</para>
        /// <para>UIDIspatcherScheduler is created when access to UIDispatcher.Default first in the whole application.</para>
        /// <para>If you want to explicitly initialize, call UIDispatcherScheduler.Initialize() in App.xaml.cs.</para>
        /// </summary>
        public static IObservable<T> ObserveOnUIDispatcher<T>(this IObservable<T> source)
        {
            var result = source.ObserveOn(UIDispatcherScheduler.Default);
            return result;
        }

        /// <summary>
        /// <para>Subscribe on UIDispatcherScheduler.</para>
        /// <para>UIDIspatcherScheduler is created when access to UIDispatcher.Default first in the whole application.</para>
        /// <para>If you want to explicitly initialize, call UIDispatcherScheduler.Initialize() in App.xaml.cs.</para>
        /// </summary>
        public static IObservable<T> SubscribeOnUIDispatcher<T>(this IObservable<T> source)
        {
            var result = source.SubscribeOn(UIDispatcherScheduler.Default);
            return result;
        }
    }
}