using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using Reactive.Bindings.Internals;

namespace Reactive.Bindings.Extensions;

/// <summary>
/// INotify Property Changed Extensions
/// </summary>
public static class INotifyPropertyChangedExtensions
{
    /// <summary>
    /// Converts PropertyChanged to an observable sequence.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static IObservable<PropertyChangedEventArgs> PropertyChangedAsObservableLegacy<T>(this T subject)
        where T : INotifyPropertyChanged =>
        InternalObservable.FromEvent<PropertyChangedEventHandler, PropertyChangedEventArgs>(
            h => (sender, e) => h(e),
            h => subject.PropertyChanged += h,
            h => subject.PropertyChanged -= h);

    /// <summary>
    /// Converts NotificationObject's property changed to an observable sequence.
    /// </summary>
    /// <typeparam name="TSubject">The type of the subject.</typeparam>
    /// <typeparam name="TProperty">The type of the property.</typeparam>
    /// <param name="subject">The subject.</param>
    /// <param name="propertySelector">Argument is self, Return is target property.</param>
    /// <param name="isPushCurrentValueAtFirst">Push current value on first subscribe.</param>
    /// <returns></returns>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static IObservable<TProperty> ObservePropertyLegacy<TSubject, TProperty>(
        this TSubject subject, Expression<Func<TSubject, TProperty>> propertySelector,
        bool isPushCurrentValueAtFirst = true)
        where TSubject : INotifyPropertyChanged
    {
        return ExpressionTreeUtils.IsNestedPropertyPath(propertySelector) ?
            ObserveNestedPropertyLegacy(subject, propertySelector, isPushCurrentValueAtFirst) :
            ObserveSimplePropertyLegacy(subject, propertySelector, isPushCurrentValueAtFirst);
    }

    private static IObservable<TProperty> ObserveSimplePropertyLegacy<TSubject, TProperty>(
        this TSubject subject, Expression<Func<TSubject, TProperty>> propertySelector,
        bool isPushCurrentValueAtFirst = true)
        where TSubject : INotifyPropertyChanged
    {
        var isFirst = true;
        var accessor = AccessorCache<TSubject>.LookupGet(propertySelector, out var propertyName);

        return Observable.Defer(() =>
        {
            var flag = isFirst;
            isFirst = false;

            var q = subject.PropertyChangedAsObservableLegacy()
                .Where(e => e.PropertyName == propertyName || string.IsNullOrEmpty(e.PropertyName))
                .Select(_ => accessor.Invoke(subject));
            return (isPushCurrentValueAtFirst && flag) ? q.StartWith(accessor.Invoke(subject)) : q;
        });
    }

    private static IObservable<TProperty> ObserveNestedPropertyLegacy<TSubject, TProperty>(
        this TSubject subject, Expression<Func<TSubject, TProperty>> propertySelector,
        bool isPushCurrentValueAtFirst = true)
        where TSubject : INotifyPropertyChanged
    {
        var propertyObserver = PropertyObservableLegacy.CreateFromPropertySelector(subject, propertySelector);
        var ox = Observable.Using(() => propertyObserver, x => x);
        return isPushCurrentValueAtFirst ?
            ox.StartWith(propertyObserver.GetPropertyPathValue()) :
            ox.AsObservable();
    }

    /// <summary>
    /// <para>Converts NotificationObject's property to ReactiveProperty. Value is two-way synchronized.</para>
    /// <para>PropertyChanged raise on ReactivePropertyScheduler.</para>
    /// </summary>
    /// <typeparam name="TSubject">The type of the subject.</typeparam>
    /// <typeparam name="TProperty">The type of the property.</typeparam>
    /// <param name="subject">The subject.</param>
    /// <param name="propertySelector">Argument is self, Return is target property.</param>
    /// <param name="mode">ReactiveProperty mode.</param>
    /// <param name="ignoreValidationErrorValue">Ignore validation error value.</param>
    /// <returns></returns>
    public static ReactiveProperty<TProperty> ToReactivePropertyAsSynchronized<TSubject, TProperty>(
        this TSubject subject,
        Expression<Func<TSubject, TProperty>> propertySelector,
        ReactivePropertyMode mode = ReactivePropertyMode.DistinctUntilChanged | ReactivePropertyMode.RaiseLatestValueOnSubscribe,
        bool ignoreValidationErrorValue = false)
        where TSubject : INotifyPropertyChanged =>
        ToReactivePropertyAsSynchronized(subject, propertySelector, ReactivePropertyScheduler.Default, mode, ignoreValidationErrorValue);

    /// <summary>
    /// <para>Converts NotificationObject's property to ReactiveProperty. Value is two-way synchronized.</para>
    /// <para>PropertyChanged raise on selected scheduler.</para>
    /// </summary>
    /// <typeparam name="TSubject">The type of the subject.</typeparam>
    /// <typeparam name="TProperty">The type of the property.</typeparam>
    /// <param name="subject">The subject.</param>
    /// <param name="propertySelector">Argument is self, Return is target property.</param>
    /// <param name="raiseEventScheduler">The raise event scheduler.</param>
    /// <param name="mode">ReactiveProperty mode.</param>
    /// <param name="ignoreValidationErrorValue">Ignore validation error value.</param>
    /// <returns></returns>
    public static ReactiveProperty<TProperty> ToReactivePropertyAsSynchronized<TSubject, TProperty>(
        this TSubject subject,
        Expression<Func<TSubject, TProperty>> propertySelector,
        IScheduler raiseEventScheduler,
        ReactivePropertyMode mode = ReactivePropertyMode.DistinctUntilChanged | ReactivePropertyMode.RaiseLatestValueOnSubscribe,
        bool ignoreValidationErrorValue = false)
        where TSubject : INotifyPropertyChanged
    {
        if (ExpressionTreeUtils.IsNestedPropertyPath(propertySelector))
        {
            var observer = PropertyObservableLegacy.CreateFromPropertySelector(subject, propertySelector);
            var result = Observable.Using(() => observer, x => x)
                .ToReactiveProperty(raiseEventScheduler, initialValue: observer.GetPropertyPathValue(), mode: mode);
            result
                .Where(_ => !ignoreValidationErrorValue || !result.HasErrors)
                .Subscribe(x => observer.SetPropertyPathValue(x));
            return result;
        }
        else
        {
            var getter = AccessorCache<TSubject>.LookupGet(propertySelector, out var _);
            var result = subject.ObserveSimplePropertyLegacy(propertySelector, isPushCurrentValueAtFirst: false)
                .ToReactiveProperty(raiseEventScheduler, initialValue: getter(subject), mode: mode);
            var setter = AccessorCache<TSubject>.LookupSet(propertySelector, out _);
            result
                .Where(_ => !ignoreValidationErrorValue || !result.HasErrors)
                .Subscribe(x => setter(subject, x));
            return result;
        }
    }

    /// <summary>
    /// <para>Converts NotificationObject's property to ReactiveProperty. Value is two-way synchronized.</para>
    /// <para>PropertyChanged raise on ReactivePropertyScheduler.</para>
    /// </summary>
    /// <typeparam name="TSubject">The type of the subject.</typeparam>
    /// <typeparam name="TProperty">The type of the property.</typeparam>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <param name="subject">The subject.</param>
    /// <param name="propertySelector">Argument is self, Return is target property.</param>
    /// <param name="convert">Convert selector to ReactiveProperty.</param>
    /// <param name="convertBack">Convert selector to source.</param>
    /// <param name="mode">ReactiveProperty mode.</param>
    /// <param name="ignoreValidationErrorValue">Ignore validation error value.</param>
    /// <returns></returns>
    public static ReactiveProperty<TResult> ToReactivePropertyAsSynchronized<TSubject, TProperty, TResult>(
        this TSubject subject,
        Expression<Func<TSubject, TProperty>> propertySelector,
        Func<TProperty, TResult> convert,
        Func<TResult, TProperty> convertBack,
        ReactivePropertyMode mode = ReactivePropertyMode.DistinctUntilChanged | ReactivePropertyMode.RaiseLatestValueOnSubscribe,
        bool ignoreValidationErrorValue = false)
        where TSubject : INotifyPropertyChanged =>
        ToReactivePropertyAsSynchronized(subject, propertySelector, convert, convertBack, ReactivePropertyScheduler.Default, mode, ignoreValidationErrorValue);

    /// <summary>
    /// <para>Converts NotificationObject's property to ReactiveProperty. Value is two-way synchronized.</para>
    /// <para>PropertyChanged raise on selected scheduler.</para>
    /// </summary>
    /// <typeparam name="TSubject">The type of the subject.</typeparam>
    /// <typeparam name="TProperty">The type of the property.</typeparam>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <param name="subject">The subject.</param>
    /// <param name="propertySelector">Argument is self, Return is target property.</param>
    /// <param name="convert">Convert selector to ReactiveProperty.</param>
    /// <param name="convertBack">Convert selector to source.</param>
    /// <param name="raiseEventScheduler">The raise event scheduler.</param>
    /// <param name="mode">ReactiveProperty mode.</param>
    /// <param name="ignoreValidationErrorValue">Ignore validation error value.</param>
    /// <returns></returns>
    public static ReactiveProperty<TResult> ToReactivePropertyAsSynchronized<TSubject, TProperty, TResult>(
        this TSubject subject,
        Expression<Func<TSubject, TProperty>> propertySelector,
        Func<TProperty, TResult> convert,
        Func<TResult, TProperty> convertBack,
        IScheduler raiseEventScheduler,
        ReactivePropertyMode mode = ReactivePropertyMode.DistinctUntilChanged | ReactivePropertyMode.RaiseLatestValueOnSubscribe,
        bool ignoreValidationErrorValue = false)
        where TSubject : INotifyPropertyChanged =>
        ToReactivePropertyAsSynchronized(subject, propertySelector,
            ox => ox.Select(convert),
            ox => ox.Select(convertBack),
            raiseEventScheduler,
            mode,
            ignoreValidationErrorValue);

    /// <summary>
    /// <para>Converts NotificationObject's property to ReactiveProperty. Value is two-way synchronized.</para>
    /// <para>PropertyChanged raise on ReactivePropertyScheduler.</para>
    /// </summary>
    /// <typeparam name="TSubject">The type of the subject.</typeparam>
    /// <typeparam name="TProperty">The type of the property.</typeparam>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <param name="subject">The subject.</param>
    /// <param name="propertySelector">Argument is self, Return is target property.</param>
    /// <param name="convert">Convert selector to ReactiveProperty.</param>
    /// <param name="convertBack">Convert selector to source.</param>
    /// <param name="mode">ReactiveProperty mode.</param>
    /// <param name="ignoreValidationErrorValue">Ignore validation error value.</param>
    /// <returns></returns>
    public static ReactiveProperty<TResult> ToReactivePropertyAsSynchronized<TSubject, TProperty, TResult>(
        this TSubject subject,
        Expression<Func<TSubject, TProperty>> propertySelector,
        Func<IObservable<TProperty>, IObservable<TResult>> convert,
        Func<IObservable<TResult>, IObservable<TProperty>> convertBack,
        ReactivePropertyMode mode = ReactivePropertyMode.DistinctUntilChanged | ReactivePropertyMode.RaiseLatestValueOnSubscribe,
        bool ignoreValidationErrorValue = false)
        where TSubject : INotifyPropertyChanged =>
        ToReactivePropertyAsSynchronized(subject, propertySelector, convert, convertBack, ReactivePropertyScheduler.Default, mode, ignoreValidationErrorValue);

    /// <summary>
    /// <para>Converts NotificationObject's property to ReactiveProperty. Value is two-way synchronized.</para>
    /// <para>PropertyChanged raise on selected scheduler.</para>
    /// </summary>
    /// <typeparam name="TSubject">The type of the subject.</typeparam>
    /// <typeparam name="TProperty">The type of the property.</typeparam>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <param name="subject">The subject.</param>
    /// <param name="propertySelector">Argument is self, Return is target property.</param>
    /// <param name="convert">Convert selector to ReactiveProperty.</param>
    /// <param name="convertBack">Convert selector to source.</param>
    /// <param name="raiseEventScheduler">The raise event scheduler.</param>
    /// <param name="mode">ReactiveProperty mode.</param>
    /// <param name="ignoreValidationErrorValue">Ignore validation error value.</param>
    /// <returns></returns>
    public static ReactiveProperty<TResult> ToReactivePropertyAsSynchronized<TSubject, TProperty, TResult>(
        this TSubject subject,
        Expression<Func<TSubject, TProperty>> propertySelector,
        Func<IObservable<TProperty>, IObservable<TResult>> convert,
        Func<IObservable<TResult>, IObservable<TProperty>> convertBack,
        IScheduler raiseEventScheduler,
        ReactivePropertyMode mode = ReactivePropertyMode.DistinctUntilChanged | ReactivePropertyMode.RaiseLatestValueOnSubscribe,
        bool ignoreValidationErrorValue = false)
        where TSubject : INotifyPropertyChanged
    {
        if (ExpressionTreeUtils.IsNestedPropertyPath(propertySelector))
        {
            var observer = PropertyObservableLegacy.CreateFromPropertySelector(subject, propertySelector);
            var result = convert(Observable.Using(() => observer, x => x)
                .StartWith(observer.GetPropertyPathValue()))
                .ToReactiveProperty<TResult>(raiseEventScheduler, mode: mode);
            convertBack(result.Where(_ => !ignoreValidationErrorValue || !result.HasErrors))
               .Subscribe(x => observer.SetPropertyPathValue(x));
            return result;
        }
        else
        {
            var setter = AccessorCache<TSubject>.LookupSet(propertySelector, out _);
            var result = convert(subject.ObservePropertyLegacy(propertySelector, isPushCurrentValueAtFirst: true))
                .ToReactiveProperty<TResult>(raiseEventScheduler, mode: mode);
            convertBack(result.Where(_ => !ignoreValidationErrorValue || !result.HasErrors))
                .Subscribe(x => setter(subject, x));
            return result;
        }
    }
}
