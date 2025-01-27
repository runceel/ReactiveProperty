using System;
using System.ComponentModel;

namespace Reactive.Bindings.Internals;

internal class PropertyChangedObservable : IObservable<PropertyChangedEventArgs>
{
    private readonly INotifyPropertyChanged _notifyPropertyChanged;

    public PropertyChangedObservable(INotifyPropertyChanged notifyPropertyChanged)
    {
        _notifyPropertyChanged = notifyPropertyChanged;
    }

    public IDisposable Subscribe(IObserver<PropertyChangedEventArgs> observer) => 
        new Subscription(_notifyPropertyChanged, observer);

    private class Subscription : IDisposable
    {
        private INotifyPropertyChanged _notifyPropertyChanged;
        private IObserver<PropertyChangedEventArgs> _observer;
        private bool _isDisposed;

        public Subscription(INotifyPropertyChanged notifyPropertyChanged, IObserver<PropertyChangedEventArgs> observer)
        {
            _notifyPropertyChanged = notifyPropertyChanged;
            _observer = observer;

            _notifyPropertyChanged.PropertyChanged += PropertyChanged;
        }

        private void PropertyChanged(object? _, PropertyChangedEventArgs e)
        {
            if (_isDisposed) return;
            _observer.OnNext(e);
        }

        public void Dispose()
        {
            if (_isDisposed) return;

            _observer.OnCompleted();
            _isDisposed = true;
            _notifyPropertyChanged!.PropertyChanged -= PropertyChanged;
            _notifyPropertyChanged = null!;
            _observer = null!;
        }
    }
}
