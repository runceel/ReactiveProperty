using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Text;
using Reactive.Bindings.Disposables;

namespace Reactive.Bindings.Internals;
internal class NestedPropertyObservable<TSubject, TProperty> : IObservable<TProperty>
    where TSubject : INotifyPropertyChanged
{
    private readonly TSubject _subject;
    private readonly Expression<Func<TSubject, TProperty>> _propertySelector;
    private readonly bool _isPushCurrentValueAtFirst;

    public NestedPropertyObservable(
        TSubject subject, 
        Expression<Func<TSubject, TProperty>> propertySelector,
        bool isPushCurrentValueAtFirst)
    {
        _subject = subject;
        _propertySelector = propertySelector;
        _isPushCurrentValueAtFirst = isPushCurrentValueAtFirst;
    }

    public IDisposable Subscribe(IObserver<TProperty> observer)
    {
        var propertyObserver = PropertyObservable.CreateFromPropertySelector(_subject, _propertySelector);
        if (_isPushCurrentValueAtFirst)
        {
            try
            {
                observer.OnNext(propertyObserver.GetPropertyPathValue());
            }
            catch (Exception ex)
            {
                observer.OnError(ex);
                propertyObserver.Dispose();
                return InternalDisposable.Empty;
            }
        }

        var disposable = propertyObserver.Subscribe(observer);
        return new CompositeDisposable(propertyObserver, disposable);
    }
}
