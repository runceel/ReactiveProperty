using System;
using System.ComponentModel;
using System.Linq.Expressions;

namespace Reactive.Bindings.Internals;
internal class SimplePropertyObservable<TSubject, TProperty> : IObservable<TProperty>
    where TSubject : INotifyPropertyChanged
{
    private readonly TSubject _subject;
    private readonly Expression<Func<TSubject, TProperty>> _propertySelector;
    private readonly bool _isPushCurrentValueAtFirst;
    private readonly Func<TSubject, TProperty> _accessor;
    private readonly string _propertyName;

    public SimplePropertyObservable(TSubject subject,
        Expression<Func<TSubject, TProperty>> propertySelector,
        bool isPushCurrentValueAtFirst = true)
    {
        _subject = subject;
        _propertySelector = propertySelector;
        _isPushCurrentValueAtFirst = isPushCurrentValueAtFirst;
        _accessor = AccessorCache<TSubject>.LookupGet(propertySelector, out _propertyName);
    }

    public IDisposable Subscribe(IObserver<TProperty> observer)
    {
        if (_isPushCurrentValueAtFirst)
        {
            try
            {
                observer.OnNext(_accessor(_subject));
            }
            catch (Exception ex)
            {
                observer.OnError(ex);
                return InternalDisposable.Empty;
            }
        }

        return new Subscription(_subject, _accessor, _propertyName, observer);
    }

    private class Subscription : IDisposable
    {
        private TSubject _subject;
        private Func<TSubject, TProperty> _accessor;
        private readonly string _propertyName;
        private IObserver<TProperty> _observer;
        private bool _isDisposed;
        private bool _onError;

        public Subscription(TSubject subject, Func<TSubject, TProperty> accessor, string propertyName, IObserver<TProperty> observer)
        {
            _subject = subject;
            _accessor = accessor;
            _propertyName = propertyName;
            _observer = observer;
            _subject.PropertyChanged += PropertyChanged;
        }

        private void PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != _propertyName && !string.IsNullOrEmpty(e.PropertyName)) return;
            if (_isDisposed) throw new InvalidOperationException();

            try
            {
                _observer.OnNext(_accessor(_subject));
            }
            catch (Exception ex)
            {
                _onError = true;
                _observer.OnError(ex);
                Dispose();
            }
        }

        public void Dispose()
        {
            if (_isDisposed) return;

            _isDisposed = true;
            _subject.PropertyChanged -= PropertyChanged;
            if (_onError is false)
            {
                try
                {
                    _observer.OnCompleted();
                }
                catch
                {
                    // noop
                }
            }

            _subject = default!;
            _observer = null!;
            _accessor = null!;
        }
    }
}
