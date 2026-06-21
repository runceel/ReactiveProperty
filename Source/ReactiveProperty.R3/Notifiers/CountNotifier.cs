using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using R3;

namespace Reactive.Bindings.R3.Notifiers;

/// <summary>
/// Notifies count change events. Backed by an R3 <see cref="Subject{T}"/>.
/// </summary>
public class CountNotifier : Observable<CountChangedStatus>, INotifyPropertyChanged
{
    private readonly object _lockObject = new();
    private readonly Subject<CountChangedStatus> _statusChanged = new();
    private readonly int _max;
    private int _count;

    /// <summary>
    /// Occurs when a property value changes.
    /// </summary>
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// Gets the maximum count.
    /// </summary>
    public int Max => _max;

    /// <summary>
    /// Gets the current count.
    /// </summary>
    public int Count
    {
        get => _count;
        private set
        {
            _count = value;
            OnPropertyChanged();
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CountNotifier"/> class.
    /// </summary>
    /// <param name="max">The maximum count value.</param>
    public CountNotifier(int max = int.MaxValue)
    {
        if (max <= 0)
        {
            throw new ArgumentException("max must be greater than 0.", nameof(max));
        }

        _max = max;
    }

    /// <summary>
    /// Increments the count and notifies the status. The returned disposable decrements the count on dispose.
    /// </summary>
    /// <param name="incrementCount">The amount to increment.</param>
    /// <returns>A disposable that decrements the count when disposed.</returns>
    public IDisposable Increment(int incrementCount = 1)
    {
        if (incrementCount < 0)
        {
            throw new ArgumentException("incrementCount must be greater than or equal to 0.", nameof(incrementCount));
        }

        lock (_lockObject)
        {
            if (Count == Max)
            {
                return Disposable.Empty;
            }
            else if (incrementCount + Count > Max)
            {
                Count = Max;
            }
            else
            {
                Count += incrementCount;
            }

            _statusChanged.OnNext(CountChangedStatus.Increment);
            if (Count == Max)
            {
                _statusChanged.OnNext(CountChangedStatus.Max);
            }

            return Disposable.Create(() => Decrement(incrementCount));
        }
    }

    /// <summary>
    /// Decrements the count and notifies the status.
    /// </summary>
    /// <param name="decrementCount">The amount to decrement.</param>
    public void Decrement(int decrementCount = 1)
    {
        if (decrementCount < 0)
        {
            throw new ArgumentException("decrementCount must be greater than or equal to 0.", nameof(decrementCount));
        }

        lock (_lockObject)
        {
            if (Count == 0)
            {
                return;
            }
            else if (Count - decrementCount < 0)
            {
                Count = 0;
            }
            else
            {
                Count -= decrementCount;
            }

            _statusChanged.OnNext(CountChangedStatus.Decrement);
            if (Count == 0)
            {
                _statusChanged.OnNext(CountChangedStatus.Empty);
            }
        }
    }

    /// <inheritdoc />
    protected override IDisposable SubscribeCore(Observer<CountChangedStatus> observer) => _statusChanged.Subscribe(observer.OnNext, observer.OnErrorResume, observer.OnCompleted);

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
