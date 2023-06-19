using System;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Threading;
using System.Windows.Threading;

namespace Reactive.Bindings.Schedulers;

/// <summary>
/// A scheduler for ReactiveProperty and ReactiveCollection on WPF.
/// </summary>
public class ReactivePropertyWpfScheduler : LocalScheduler
{
    private readonly Dispatcher _dispatcher;
    private readonly DispatcherSynchronizationContext _context;

    /// <summary>
    /// Construct a scheduler from Dispatcher.
    /// </summary>
    /// <param name="dispatcher">An application's dispatcher</param>
    public ReactivePropertyWpfScheduler(Dispatcher dispatcher)
    {
        _dispatcher = dispatcher;
        _context = new DispatcherSynchronizationContext(dispatcher);
    }

    /// <summary>
    /// Schedules an action to be executed.
    /// </summary>
    /// <typeparam name="TState">The type of the state passed to the scheduled action.</typeparam>
    /// <param name="state">State passed to the action to be executed.</param>
    /// <param name="action">Action to be executed.</param>
    /// <returns>The disposable object used to cancel the scheduled action (best effort).</returns>
    public override IDisposable Schedule<TState>(TState state, Func<IScheduler, TState, IDisposable> action)
    {
        if (action == null)
        {
            throw new ArgumentNullException(nameof(action));
        }

        if (_dispatcher.CheckAccess())
        {
            return action(this, state);
        }

        var d = new SingleAssignmentDisposable();

        _context.PostWithStartComplete(() =>
        {
            if (!d.IsDisposed)
            {
                d.Disposable = action(this, state);
            }
        });

        return d;
    }

    /// <summary>
    /// Schedules an action to be executed after dueTime.
    /// </summary>
    /// <typeparam name="TState">The type of the state passed to the scheduled action.</typeparam>
    /// <param name="state">State passed to the action to be executed.</param>
    /// <param name="action">Action to be executed.</param>
    /// <param name="dueTime">Relative time after which to execute the action.</param>
    /// <returns>The disposable object used to cancel the scheduled action (best effort).</returns>
    public override IDisposable Schedule<TState>(TState state, TimeSpan dueTime, Func<IScheduler, TState, IDisposable> action)
    {
        if (action == null)
        {
            throw new ArgumentNullException(nameof(action));
        }

        var dt = Scheduler.Normalize(dueTime);
        if (dt.Ticks == 0)
        {
            return Schedule(state, action);
        }

        // Note that avoiding closure allocation here would introduce infinite generic recursion over the TState argument
        return DefaultScheduler.Instance.Schedule(state, dt, (_, state1) => Schedule(state1, action));
    }
}
internal static class SynchronizationContextExtensions
{
    public static void PostWithStartComplete<T>(this SynchronizationContext context, Action<T> action, T state)
    {
        context.OperationStarted();

        context.Post(
            o =>
            {
                try
                {
                    action((T)o!);
                }
                finally
                {
                    context.OperationCompleted();
                }
            },
            state
        );
    }

    public static void PostWithStartComplete(this SynchronizationContext context, Action action)
    {
        context.OperationStarted();

        context.Post(
            _ =>
            {
                try
                {
                    action();
                }
                finally
                {
                    context.OperationCompleted();
                }
            },
            null
        );
    }
}
