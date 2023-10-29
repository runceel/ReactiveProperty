using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Windows.Threading;

namespace Reactive.Bindings.Schedulers;

/// <summary>
/// A scheduler for ReactiveProperty and ReactiveCollection on WPF.
/// </summary>
public class ReactivePropertyWpfScheduler : IScheduler
{
    private readonly IScheduler _scheduler;
    private readonly Dispatcher _dispatcher;

    /// <summary>
    /// Gets the scheduler's notion of current time.
    /// </summary>
    public DateTimeOffset Now => DateTimeOffset.Now;

    /// <summary>
    /// Construct a scheduler from Dispatcher.
    /// </summary>
    /// <param name="dispatcher">An application's dispatcher</param>
    public ReactivePropertyWpfScheduler(Dispatcher dispatcher)
    {
        if (dispatcher == null) throw new ArgumentNullException(nameof(dispatcher));

        _dispatcher = dispatcher;
        _scheduler = new SynchronizationContextScheduler(new DispatcherSynchronizationContext(dispatcher));
    }

    /// <summary>
    /// Schedules an action to be executed.
    /// </summary>
    /// <typeparam name="TState">The type of the state passed to the scheduled action.</typeparam>
    /// <param name="state">State passed to the action to be executed.</param>
    /// <param name="action">Action to be executed.</param>
    /// <returns>The disposable object used to cancel the scheduled action (best effort).</returns>
    /// <exception cref="ArgumentNullException"><paramref name="action"/> is <c>null</c>.</exception>
    public IDisposable Schedule<TState>(TState state, Func<IScheduler, TState, IDisposable> action)
    {
        if (action == null)
        {
            throw new ArgumentNullException(nameof(action));
        }

        if (_dispatcher.CheckAccess())
        {
            return action(this, state);
        }

        return _scheduler.Schedule(state, action);
    }

    /// <summary>
    /// Schedules an action to be executed after dueTime.
    /// </summary>
    /// <typeparam name="TState">The type of the state passed to the scheduled action.</typeparam>
    /// <param name="state">State passed to the action to be executed.</param>
    /// <param name="action">Action to be executed.</param>
    /// <param name="dueTime">Relative time after which to execute the action.</param>
    /// <returns>The disposable object used to cancel the scheduled action (best effort).</returns>
    /// <exception cref="ArgumentNullException"><paramref name="action"/> is <c>null</c>.</exception>
    public IDisposable Schedule<TState>(TState state, TimeSpan dueTime, Func<IScheduler, TState, IDisposable> action)
    {
        if (action == null)
        {
            throw new ArgumentNullException(nameof(action));
        }

        var normalized = Scheduler.Normalize(dueTime);
        if (normalized.Ticks == 0)
        {
            return Schedule(state, action);
        }

        return _scheduler.Schedule(state, dueTime, action);
    }

    /// <summary>
    /// Schedules an action to be executed at dueTime.
    /// </summary>
    /// <typeparam name="TState">The type of the state passed to the scheduled action.</typeparam>
    /// <param name="state">State passed to the action to be executed.</param>
    /// <param name="action">Action to be executed.</param>
    /// <param name="dueTime">Absolute time at which to execute the action.</param>
    /// <returns>The disposable object used to cancel the scheduled action (best effort).</returns>
    /// <exception cref="ArgumentNullException"><paramref name="action"/> is <c>null</c>.</exception>
    public IDisposable Schedule<TState>(TState state, DateTimeOffset dueTime, Func<IScheduler, TState, IDisposable> action)
    {
        if (action == null)
        {
            throw new ArgumentNullException(nameof(action));
        }

        return Schedule(state, dueTime - Now, action);
    }
}

