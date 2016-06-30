using System;
using System.Reactive.Concurrency;
using System.Threading;

namespace Reactive.Bindings
{
    /// <summary>
    /// <para>If call Schedule on UIThread then schedule immediately else dispatch BeginInvoke.</para>
    /// <para>UIDIspatcherScheduler is created when access to UIDispatcher.Default first in the whole application.</para>
    /// <para>If you want to explicitly initialize, call UIDispatcherScheduler.Initialize() in App.xaml.cs.</para>
    /// </summary>
    public static class UIDispatcherScheduler
    {
        private static Lazy<SynchronizationContextScheduler> DefaultScheduler { get; } =
            new Lazy<SynchronizationContextScheduler>(() =>
                {
                    if (SynchronizationContext.Current == null)
                    {
                        throw new InvalidOperationException("SynchronizationContext.Current is null");
                    }

                    return new SynchronizationContextScheduler(SynchronizationContext.Current);
                });

        /// <summary>
        /// <para>If call Schedule on UIThread then schedule immediately else dispatch BeginInvoke.</para>
        /// <para>UIDIspatcherScheduler is created when access to UIDispatcher.Default first in the whole application.</para>
        /// <para>If you want to explicitly initialize, call UIDispatcherScheduler.Initialize() in App.xaml.cs.</para>
        /// </summary>
        public static IScheduler Default => DefaultScheduler.Value;

        internal static bool IsSchedulerCreated = DefaultScheduler.IsValueCreated;

        /// <summary>
        /// Create UIDispatcherSchedule on called thread if is not initialized yet.
        /// </summary>
        public static void Initialize()
        {
            var _ = DefaultScheduler.Value;
        }

    }
}