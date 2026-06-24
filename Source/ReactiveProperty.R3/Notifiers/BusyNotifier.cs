using System;
using System.ComponentModel;
using R3;

namespace Reactive.Bindings.R3.Notifiers;

/// <summary>
/// Notifies a busy state via a thread-safe reference count. Backed by an R3 <see cref="Subject{T}"/>.
/// </summary>
public class BusyNotifier : Observable<bool>, INotifyPropertyChanged
{
    private static readonly PropertyChangedEventArgs s_isBusyPropertyChangedEventArgs = new(nameof(IsBusy));

    private readonly object _lockObject = new();
    private readonly Subject<bool> _isBusySubject = new();
    private int _processCounter;
    private bool _isBusy;

    /// <summary>
    /// Occurs when a property value changes.
    /// </summary>
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// Gets a value indicating whether a process is running.
    /// </summary>
    public bool IsBusy
    {
        get => _isBusy;
        private set
        {
            if (_isBusy == value)
            {
                return;
            }

            _isBusy = value;
            PropertyChanged?.Invoke(this, s_isBusyPropertyChangedEventArgs);
            _isBusySubject.OnNext(_isBusy);
        }
    }

    /// <summary>
    /// Starts a process. Dispose the returned value when the process ends.
    /// </summary>
    /// <returns>A disposable that decrements the reference count when disposed.</returns>
    public IDisposable ProcessStart()
    {
        lock (_lockObject)
        {
            _processCounter++;
            IsBusy = _processCounter != 0;
            return Disposable.Create(() =>
            {
                lock (_lockObject)
                {
                    _processCounter--;
                    IsBusy = _processCounter != 0;
                }
            });
        }
    }

    /// <inheritdoc />
    protected override IDisposable SubscribeCore(Observer<bool> observer)
    {
        observer.OnNext(IsBusy);
        return _isBusySubject.Subscribe(observer.OnNext, observer.OnErrorResume, observer.OnCompleted);
    }
}
