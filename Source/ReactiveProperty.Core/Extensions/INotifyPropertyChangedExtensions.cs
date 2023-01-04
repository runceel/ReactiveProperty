using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using Reactive.Bindings.Internals;
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
