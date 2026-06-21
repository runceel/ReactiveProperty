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
        ITimer? timer = null;
        timer = _timeProvider.CreateTimer(
            _ =>
            {
                _trigger.OnNext(value);
                timer?.Dispose();
            },
            null,
            dueTime,
            Timeout.InfiniteTimeSpan);
        return timer;
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
}
