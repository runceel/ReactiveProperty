using System;
using System.ComponentModel;
using System.Threading;
using R3;

namespace Reactive.Bindings.R3;

/// <summary>
/// A hot, stoppable/restartable observable timer backed by a <see cref="TimeProvider"/>.
/// Emits an incrementing <see cref="long"/> on each tick.
/// </summary>
public class ReactiveTimer : Observable<long>, INotifyPropertyChanged, IDisposable
{
    private static readonly PropertyChangedEventArgs s_isEnabledPropertyChangedEventArgs = new(nameof(IsEnabled));
    private static readonly PropertyChangedEventArgs s_intervalPropertyChangedEventArgs = new(nameof(Interval));

    private readonly object _lockObject = new();
    private readonly TimeProvider _timeProvider;
    private readonly Subject<long> _subject = new();
    private ITimer? _timer;
    private long _count;
    private bool _isDisposed;
    private TimeSpan _interval;
    private bool _isEnabled;

    /// <summary>
    /// Occurs when a property value changes.
    /// </summary>
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// Initializes a new instance of the <see cref="ReactiveTimer"/> class.
    /// </summary>
    /// <param name="interval">The timer interval.</param>
    /// <param name="timeProvider">
    /// The time provider used to schedule ticks. Defaults to <see cref="ObservableSystem.DefaultTimeProvider"/>.
    /// </param>
    public ReactiveTimer(TimeSpan interval, TimeProvider? timeProvider = null)
    {
        _interval = interval;
        _timeProvider = timeProvider ?? ObservableSystem.DefaultTimeProvider;
    }

    /// <summary>
    /// Gets or sets the timer interval.
    /// </summary>
    public TimeSpan Interval
    {
        get
        {
            lock (_lockObject)
            {
                return _interval;
            }
        }

        set
        {
            lock (_lockObject)
            {
                if (_interval == value)
                {
                    return;
                }

                _interval = value;
            }

            PropertyChanged?.Invoke(this, s_intervalPropertyChangedEventArgs);
        }
    }

    /// <summary>
    /// Gets a value indicating whether this instance is enabled.
    /// </summary>
    public bool IsEnabled
    {
        get => _isEnabled;
        private set
        {
            if (_isEnabled == value)
            {
                return;
            }

            _isEnabled = value;
            PropertyChanged?.Invoke(this, s_isEnabledPropertyChangedEventArgs);
        }
    }

    /// <summary>
    /// Starts the timer immediately.
    /// </summary>
    public void Start() => Start(TimeSpan.Zero);

    /// <summary>
    /// Starts the timer after the specified due time.
    /// </summary>
    /// <param name="dueTime">The delay before the first tick.</param>
    public void Start(TimeSpan dueTime)
    {
        lock (_lockObject)
        {
            if (_isDisposed)
            {
                return;
            }

            _timer?.Dispose();
            _timer = _timeProvider.CreateTimer(OnTick, null, dueTime, _interval);
        }

        IsEnabled = true;
    }

    /// <summary>
    /// Stops the timer.
    /// </summary>
    public void Stop()
    {
        lock (_lockObject)
        {
            _timer?.Dispose();
            _timer = null;
        }

        IsEnabled = false;
    }

    /// <summary>
    /// Stops the timer and resets the count.
    /// </summary>
    public void Reset()
    {
        lock (_lockObject)
        {
            _timer?.Dispose();
            _timer = null;
            _count = 0;
        }

        IsEnabled = false;
    }

    /// <inheritdoc />
    protected override IDisposable SubscribeCore(Observer<long> observer) => _subject.Subscribe(observer.OnNext, observer.OnErrorResume, observer.OnCompleted);

    /// <summary>
    /// Sends OnCompleted to subscribers, unsubscribes all, and stops the timer.
    /// </summary>
    public void Dispose()
    {
        lock (_lockObject)
        {
            if (_isDisposed)
            {
                return;
            }

            _isDisposed = true;
            _timer?.Dispose();
            _timer = null;
        }

        _subject.OnCompleted();
        _subject.Dispose();
    }

    private void OnTick(object? state)
    {
        lock (_lockObject)
        {
            if (_isDisposed || _timer is null)
            {
                return;
            }

            _subject.OnNext(_count++);
        }
    }
}
