using System;
using System.Threading;
using R3;

namespace Reactive.Bindings.R3.Notifiers;

/// <summary>
/// Notifies a value immediately or after a delay using a <see cref="TimeProvider"/>.
/// Backed by an R3 <see cref="Subject{T}"/>.
/// </summary>
/// <typeparam name="T">The value type.</typeparam>
public class ScheduledNotifier<T> : Observable<T>, IProgress<T>
{
    private readonly TimeProvider _timeProvider;
    private readonly Subject<T> _trigger = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="ScheduledNotifier{T}"/> class.
    /// </summary>
    /// <param name="timeProvider">
    /// The time provider used to schedule delayed reports. Defaults to
    /// <see cref="ObservableSystem.DefaultTimeProvider"/>.
    /// </param>
    public ScheduledNotifier(TimeProvider? timeProvider = null)
    {
        _timeProvider = timeProvider ?? ObservableSystem.DefaultTimeProvider;
    }

    /// <summary>
    /// Pushes a value to subscribers immediately.
    /// </summary>
    /// <param name="value">The value to report.</param>
    public void Report(T value) => _trigger.OnNext(value);

    /// <summary>
    /// Pushes a value to subscribers after the specified delay.
    /// </summary>
    /// <param name="value">The value to report.</param>
    /// <param name="dueTime">The delay before the value is reported.</param>
    /// <returns>A disposable that cancels the scheduled report when disposed.</returns>
    public IDisposable Report(T value, TimeSpan dueTime)
    {
        // A holder is used so the callback can always dispose the ITimer safely,
        // even if the timer fires before the assignment to the local variable completes
        // (e.g. dueTime == Zero on an eager TimeProvider).
        var holder = new SingleFireTimerHolder();
        var timer = _timeProvider.CreateTimer(
            _ =>
            {
                _trigger.OnNext(value);
                holder.Dispose();
            },
            null,
            dueTime,
            Timeout.InfiniteTimeSpan);
        holder.SetTimer(timer);
        return holder;
    }

    /// <summary>
    /// Pushes a value to subscribers at the specified point in time.
    /// </summary>
    /// <param name="value">The value to report.</param>
    /// <param name="dueTime">The absolute time at which the value is reported.</param>
    /// <returns>A disposable that cancels the scheduled report when disposed.</returns>
    public IDisposable Report(T value, DateTimeOffset dueTime)
    {
        var delay = dueTime - _timeProvider.GetUtcNow();
        if (delay < TimeSpan.Zero)
        {
            delay = TimeSpan.Zero;
        }

        return Report(value, delay);
    }

    /// <inheritdoc />
    protected override IDisposable SubscribeCore(Observer<T> observer) => _trigger.Subscribe(observer.OnNext, observer.OnErrorResume, observer.OnCompleted);

    /// <summary>
    /// Thread-safe wrapper that holds an <see cref="ITimer"/> and guarantees the timer is
    /// disposed exactly once regardless of whether <see cref="SetTimer"/> or
    /// <see cref="Dispose"/> is called first.
    /// </summary>
    private sealed class SingleFireTimerHolder : IDisposable
    {
        private ITimer? _timer;
        private bool _disposeRequested;
        private readonly object _gate = new();

        public void SetTimer(ITimer timer)
        {
            lock (_gate)
            {
                _timer = timer;
                if (!_disposeRequested)
                {
                    return;
                }
            }

            // Dispose was already requested before the timer was assigned.
            timer.Dispose();
        }

        public void Dispose()
        {
            ITimer? toDispose;
            lock (_gate)
            {
                if (_disposeRequested)
                {
                    return;
                }

                _disposeRequested = true;
                toDispose = _timer;
                _timer = null;
            }

            toDispose?.Dispose();
        }
    }
}
