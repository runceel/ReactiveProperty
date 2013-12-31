using System;
using System.Windows;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reactive.Disposables;
using System.Reactive.Concurrency;
using System.Threading;

namespace Codeplex.Reactive
{
    /// <summary>
    /// <para>If call Schedule on UIThread then schedule immediately else dispatch BeginInvoke.</para>
    /// <para>UIDIspatcherScheduler is created when access to UIDispatcher.Default first in the whole application.</para>
    /// <para>If you want to explicitly initialize, call UIDispatcherScheduler.Initialize() in App.xaml.cs.</para>
    /// </summary>
    public static class UIDispatcherScheduler
    {
        private static readonly Lazy<SynchronizationContextScheduler> defaultScheduler =
            new Lazy<SynchronizationContextScheduler>(() => new SynchronizationContextScheduler(SynchronizationContext.Current));

        /// <summary>
        /// <para>If call Schedule on UIThread then schedule immediately else dispatch BeginInvoke.</para>
        /// <para>UIDIspatcherScheduler is created when access to UIDispatcher.Default first in the whole application.</para>
        /// <para>If you want to explicitly initialize, call UIDispatcherScheduler.Initialize() in App.xaml.cs.</para>
        /// </summary>
        public static IScheduler Default
        {
            get 
            {
                return defaultScheduler.Value; 
            }
        }

        /// <summary>
        /// Create UIDispatcherSchedule on called thread if is not initialized yet.
        /// </summary>
        public static void Initialize()
        {
            var _ = defaultScheduler.Value;
        }

    }
}