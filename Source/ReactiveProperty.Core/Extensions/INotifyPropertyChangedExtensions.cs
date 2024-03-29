﻿using System;
using System.ComponentModel;
using Reactive.Bindings.Internals;
using Reactive.Bindings.TinyLinq;
using System.Linq.Expressions;

namespace Reactive.Bindings.Extensions;

/// <summary>
/// Extension methods for <see cref="INotifyPropertyChanged"/>
/// </summary>
public static class INotifyPropertyChangedExtensions
{
    /// <summary>
    /// Converts PropertyChanged to an observable sequence.
    /// </summary>
    public static IObservable<PropertyChangedEventArgs> PropertyChangedAsObservable<T>(this T subject)
        where T : INotifyPropertyChanged =>
        new PropertyChangedObservable(subject);

    /// <summary>
    /// Converts NotificationObject's property changed to an observable sequence.
    /// </summary>
    /// <typeparam name="TSubject">The type of the subject.</typeparam>
    /// <typeparam name="TProperty">The type of the property.</typeparam>
    /// <param name="subject">The subject.</param>
    /// <param name="propertySelector">Argument is self, Return is target property.</param>
    /// <param name="isPushCurrentValueAtFirst">Push current value on first subscribe.</param>
    /// <returns></returns>
    public static IObservable<TProperty> ObserveProperty<TSubject, TProperty>(
        this TSubject subject, Expression<Func<TSubject, TProperty>> propertySelector,
        bool isPushCurrentValueAtFirst = true)
        where TSubject : INotifyPropertyChanged
    {
        return ExpressionTreeUtils.IsNestedPropertyPath(propertySelector) ?
            ObserveNestedProperty(subject, propertySelector, isPushCurrentValueAtFirst) :
            ObserveSimpleProperty(subject, propertySelector, isPushCurrentValueAtFirst);
    }

    /// <summary>
    /// <para>Converts NotificationObject's property to ReactivePropertySlim. Value is two-way synchronized.</para>
    /// <para>PropertyChanged raise on selected scheduler.</para>
    /// </summary>
    /// <typeparam name="TSubject">The type of the subject.</typeparam>
    /// <typeparam name="TProperty">The type of the property.</typeparam>
    /// <param name="subject">The subject.</param>
    /// <param name="propertySelector">Argument is self, Return is target property.</param>
    /// <param name="mode">ReactiveProperty mode.</param>
    /// <returns></returns>
    public static ReactivePropertySlim<TProperty> ToReactivePropertySlimAsSynchronized<TSubject, TProperty>(
        this TSubject subject,
        Expression<Func<TSubject, TProperty>> propertySelector,
        ReactivePropertyMode mode = ReactivePropertyMode.DistinctUntilChanged | ReactivePropertyMode.RaiseLatestValueOnSubscribe)
        where TSubject : INotifyPropertyChanged
    {
        if (ExpressionTreeUtils.IsNestedPropertyPath(propertySelector))
        {
            var observer = PropertyObservable.CreateFromPropertySelector(subject, propertySelector);
            var result = new ReactivePropertySlim<TProperty>(observer.GetPropertyPathValue(), mode: mode);
            var disposable = observer.Subscribe(new DelegateObserver<TProperty>(x => result.Value = x));
            result.Subscribe(new DelegateObserver<TProperty>(x => observer.SetPropertyPathValue(x), _ => disposable.Dispose(), () => disposable.Dispose()));
            return result;
        }
        else
        {
            var setter = AccessorCache<TSubject>.LookupSet(propertySelector, out _);
            var result = new ReactivePropertySlim<TProperty>(mode: mode);
            var disposable = subject.ObserveProperty(propertySelector, isPushCurrentValueAtFirst: true)
                .Subscribe(new DelegateObserver<TProperty>(x => result.Value = x));
            result.Subscribe(new DelegateObserver<TProperty>(x => setter(subject, x), _ => disposable.Dispose(), () => disposable.Dispose()));
            return result;
        }
    }

    /// <summary>
    /// <para>Converts NotificationObject's property to ReactivePropertySlim. Value is two-way synchronized.</para>
    /// <para>PropertyChanged raise on selected scheduler.</para>
    /// </summary>
    /// <typeparam name="TSubject">The type of the subject.</typeparam>
    /// <typeparam name="TProperty">The type of the property.</typeparam>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <param name="subject">The subject.</param>
    /// <param name="propertySelector">Argument is self, Return is target property.</param>
    /// <param name="convert">Convert selector to ReactiveProperty.</param>
    /// <param name="convertBack">Convert selector to source.</param>
    /// <param name="mode">ReactiveProperty mode.</param>
    /// <returns></returns>
    public static ReactivePropertySlim<TResult> ToReactivePropertySlimAsSynchronized<TSubject, TProperty, TResult>(
        this TSubject subject,
        Expression<Func<TSubject, TProperty>> propertySelector,
        Func<TProperty, TResult> convert,
        Func<TResult, TProperty> convertBack,
        ReactivePropertyMode mode = ReactivePropertyMode.DistinctUntilChanged | ReactivePropertyMode.RaiseLatestValueOnSubscribe)
        where TSubject : INotifyPropertyChanged =>
        ToReactivePropertySlimAsSynchronized(subject, propertySelector,
            ox => ox.Select(convert),
            ox => ox.Select(convertBack),
            mode);

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
    /// <param name="mode">ReactiveProperty mode.</param>
    /// <returns></returns>
    public static ReactivePropertySlim<TResult> ToReactivePropertySlimAsSynchronized<TSubject, TProperty, TResult>(
        this TSubject subject,
        Expression<Func<TSubject, TProperty>> propertySelector,
        Func<IObservable<TProperty>, IObservable<TResult>> convert,
        Func<IObservable<TResult>, IObservable<TProperty>> convertBack,
        ReactivePropertyMode mode = ReactivePropertyMode.DistinctUntilChanged | ReactivePropertyMode.RaiseLatestValueOnSubscribe)
        where TSubject : INotifyPropertyChanged
    {
        if (ExpressionTreeUtils.IsNestedPropertyPath(propertySelector))
        {
            var result = new ReactivePropertySlim<TResult>(mode: mode);
            var observer = PropertyObservable.CreateFromPropertySelector(subject, propertySelector);
            var disposable = convert(subject.ObserveProperty(propertySelector, isPushCurrentValueAtFirst: true))
                .Subscribe(new DelegateObserver<TResult>(x => result.Value = x));
            convertBack(result)
                .Subscribe(new DelegateObserver<TProperty>(
                    x => observer.SetPropertyPathValue(x), 
                    _ => disposable.Dispose(), 
                    () => disposable.Dispose()));
            return result;
        }
        else
        {
            var setter = AccessorCache<TSubject>.LookupSet(propertySelector, out _);
            var result = new ReactivePropertySlim<TResult>(mode: mode);
            var disposable = convert(subject.ObserveProperty(propertySelector, isPushCurrentValueAtFirst: true))
                .Subscribe(new DelegateObserver<TResult>(x => result.Value = x));
            convertBack(result)
                .Subscribe(new DelegateObserver<TProperty>(
                    x => setter(subject, x), 
                    _ => disposable.Dispose(), 
                    () => disposable.Dispose()));
            return result;
        }
    }


    internal static IObservable<TProperty> ObserveSimpleProperty<TSubject, TProperty>(
        this TSubject subject, Expression<Func<TSubject, TProperty>> propertySelector,
        bool isPushCurrentValueAtFirst = true)
        where TSubject : INotifyPropertyChanged => 
        new SimplePropertyObservable<TSubject, TProperty>(subject, propertySelector, isPushCurrentValueAtFirst);

    internal static IObservable<TProperty> ObserveNestedProperty<TSubject, TProperty>(
        this TSubject subject, Expression<Func<TSubject, TProperty>> propertySelector,
        bool isPushCurrentValueAtFirst = true)
        where TSubject : INotifyPropertyChanged => 
        new NestedPropertyObservable<TSubject, TProperty>(subject, propertySelector, isPushCurrentValueAtFirst);
}
