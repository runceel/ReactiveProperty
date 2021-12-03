using System;
using System.Reactive.Concurrency;
using System.Threading;

namespace Reactive.Bindings;

/// <summary>
/// Default ReactiveProperty Scheduler provider.
/// </summary>
public static class ReactivePropertyScheduler
{
    private static Func<IScheduler> defaultSchedulerFactory;

    /// <summary>
    /// Get ReactiveProperty default scheduler.
    /// </summary>
    public static IScheduler Default
    {
        get
        {
            if (defaultSchedulerFactory != null)
            {
                return defaultSchedulerFactory();
            }
            if (UIDispatcherScheduler.IsSchedulerCreated)
            {
                return UIDispatcherScheduler.Default;
            }

            if (SynchronizationContext.Current == null)
            {
                return ImmediateScheduler.Instance;
            }

            return UIDispatcherScheduler.Default;
        }
    }

    /// <summary>
    /// Set default scheduler.
    /// </summary>
    /// <param name="defaultScheduler"></param>
    public static void SetDefault(IScheduler defaultScheduler)
    {
        SetDefaultSchedulerFactory(() => defaultScheduler);
    }

    /// <summary>
    /// Set default scheduler factory,
    /// </summary>
    /// <param name="schedulerFactory"></param>
    public static void SetDefaultSchedulerFactory(Func<IScheduler> schedulerFactory)
    {
        defaultSchedulerFactory = schedulerFactory;
    }
}
