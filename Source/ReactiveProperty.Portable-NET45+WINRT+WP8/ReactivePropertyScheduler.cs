using System;
using System.Reactive.Concurrency;

namespace Reactive.Bindings
{
    /// <summary>
    /// Default ReactiveProperty Scheduler provider.
    /// </summary>
    public static class ReactivePropertyScheduler
    {
        private static IScheduler DefaultScheduler { get; set; }

        /// <summary>
        /// Get ReactiveProperty default scheduler.
        /// </summary>
        public static IScheduler Default => DefaultScheduler ?? UIDispatcherScheduler.Default;

        /// <summary>
        /// set default scheduler.
        /// </summary>
        /// <param name="defaultScheduler"></param>
        public static void SetDefault(IScheduler defaultScheduler)
        {
            DefaultScheduler = defaultScheduler;
        }
    }
}
