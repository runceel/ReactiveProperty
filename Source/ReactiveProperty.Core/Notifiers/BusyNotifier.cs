using System;
using System.ComponentModel;
using Reactive.Bindings.Internals;

namespace Reactive.Bindings.Notifiers;

/// <summary>
/// Notify of busy.
/// </summary>
public class BusyNotifier : INotifyPropertyChanged, IObservable<bool>
{

    private static readonly PropertyChangedEventArgs IsBusyPropertyChangedEventArgs = new(nameof(IsBusy));

    /// <summary>
    /// property changed event handler
    /// </summary>
    public event PropertyChangedEventHandler? PropertyChanged;

    private int ProcessCounter { get; set; }

    private InternalSubject<bool> IsBusySubject { get; } = new ();

    private object LockObject { get; } = new object();

    private bool isBusy;

    /// <summary>
    /// Is process running.
    /// </summary>
    public bool IsBusy
    {
        get { return isBusy; }
        set
        {
            if (isBusy == value) { return; }
            isBusy = value;
            PropertyChanged?.Invoke(this, IsBusyPropertyChangedEventArgs);
            IsBusySubject.OnNext(isBusy);
        }
    }

    /// <summary>
    /// Process start.
    /// </summary>
    /// <returns>Call dispose method when process end.</returns>
    public IDisposable ProcessStart()
    {
        lock (LockObject)
        {
            ProcessCounter++;
            IsBusy = ProcessCounter != 0;
            return new DelegateDisposable(() =>
            {
                lock (LockObject)
                {
                    ProcessCounter--;
                    IsBusy = ProcessCounter != 0;
                }
            });
        }
    }

    /// <summary>
    /// Subscribe busy.
    /// </summary>
    /// <param name="observer">observer</param>
    /// <returns>disposable</returns>
    public IDisposable Subscribe(IObserver<bool> observer)
    {
        observer.OnNext(IsBusy);
        return IsBusySubject.Subscribe(observer);
    }
}
