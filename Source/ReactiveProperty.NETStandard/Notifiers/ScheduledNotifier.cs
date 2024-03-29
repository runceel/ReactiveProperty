﻿using System;
using System.Reactive.Concurrency;
using System.Reactive.Subjects;

namespace Reactive.Bindings.Notifiers;

/// <summary>
/// Notify value on provided scheduler.
/// </summary>
public class ScheduledNotifier<T> : IObservable<T>, IProgress<T>
{
    readonly IScheduler scheduler;
    readonly Subject<T> trigger = new();

    /// <summary>
    /// Use scheduler is Scheduler.Immediate.
    /// </summary>
    public ScheduledNotifier()
    {
        scheduler = Scheduler.Immediate;
    }
    /// <summary>
    /// Use scheduler is argument's scheduler.
    /// </summary>
    public ScheduledNotifier(IScheduler scheduler)
    {
        this.scheduler = scheduler ?? throw new ArgumentNullException(nameof(scheduler));
    }

    /// <summary>
    /// Push value to subscribers on provided scheduler.
    /// </summary>
    public void Report(T value) => scheduler.Schedule(() => trigger.OnNext(value));

    /// <summary>
    /// Push value to subscribers on provided scheduler.
    /// </summary>
    public IDisposable Report(T value, TimeSpan dueTime) => scheduler.Schedule(dueTime, () => trigger.OnNext(value));

    /// <summary>
    /// Push value to subscribers on provided scheduler.
    /// </summary>
    public IDisposable Report(T value, DateTimeOffset dueTime) => scheduler.Schedule(dueTime, () => trigger.OnNext(value));

    /// <summary>
    /// Subscribe observer.
    /// </summary>
    public IDisposable Subscribe(IObserver<T> observer)
    {
        if (observer == null)
        {
            throw new ArgumentNullException(nameof(observer));
        }

        return trigger.Subscribe(observer);
    }
}
