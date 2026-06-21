using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using R3;

namespace Reactive.Bindings.R3.Extensions;

/// <summary>
/// Provides R3 observable helpers for <see cref="INotifyDataErrorInfo"/>.
/// </summary>
public static class INotifyDataErrorInfoExtensions
{
    /// <summary>
    /// Converts <see cref="INotifyDataErrorInfo.ErrorsChanged"/> to an observable sequence.
    /// </summary>
    public static Observable<DataErrorsChangedEventArgs> ErrorsChangedAsObservable<T>(this T subject)
        where T : INotifyDataErrorInfo
    {
        if (subject is null)
        {
            throw new ArgumentNullException(nameof(subject));
        }

        return Observable.FromEvent<EventHandler<DataErrorsChangedEventArgs>, DataErrorsChangedEventArgs>(
            h => (_, e) => h(e),
            h => subject.ErrorsChanged += h,
            h => subject.ErrorsChanged -= h,
            default);
    }

    /// <summary>
    /// Observes <see cref="INotifyDataErrorInfo.HasErrors"/>.
    /// </summary>
    public static Observable<bool> ObserveHasErrors(this INotifyDataErrorInfo subject)
    {
        if (subject is null)
        {
            throw new ArgumentNullException(nameof(subject));
        }

        return Observable.Create<bool>(observer =>
        {
            observer.OnNext(subject.HasErrors);
            return subject.ErrorsChangedAsObservable().Subscribe(_ => observer.OnNext(subject.HasErrors));
        });
    }

    /// <summary>
    /// Observes current errors for the Value property.
    /// </summary>
    public static Observable<IReadOnlyList<string>> ObserveErrorChanged(this INotifyDataErrorInfo subject) =>
        subject.ObserveErrorChanged("Value");

    /// <summary>
    /// Observes current errors for the specified property.
    /// </summary>
    public static Observable<IReadOnlyList<string>> ObserveErrorChanged(this INotifyDataErrorInfo subject, string? propertyName)
    {
        if (subject is null)
        {
            throw new ArgumentNullException(nameof(subject));
        }

        return Observable.Create<IReadOnlyList<string>>(observer =>
        {
            observer.OnNext(GetErrors(subject, propertyName));
            return subject.ErrorsChangedAsObservable()
                .Where(e => string.IsNullOrEmpty(propertyName) || e.PropertyName == propertyName)
                .Subscribe(_ => observer.OnNext(GetErrors(subject, propertyName)));
        });
    }

    /// <summary>
    /// Observes current errors for the selected property.
    /// </summary>
    public static Observable<IReadOnlyList<string>> ObserveErrorInfo<TSubject, TProperty>(
        this TSubject subject,
        Expression<Func<TSubject, TProperty>> propertySelector)
        where TSubject : INotifyDataErrorInfo
    {
        if (propertySelector is null)
        {
            throw new ArgumentNullException(nameof(propertySelector));
        }

        if (propertySelector.Body is not MemberExpression memberExpression || memberExpression.Member is not System.Reflection.PropertyInfo)
        {
            throw new ArgumentException("The selector must point to a property.", nameof(propertySelector));
        }

        return subject.ObserveErrorChanged(memberExpression.Member.Name);
    }

    private static IReadOnlyList<string> GetErrors(INotifyDataErrorInfo subject, string? propertyName)
    {
        var errors = subject.GetErrors(propertyName);
        if (errors is null)
        {
            return Array.Empty<string>();
        }

        if (errors is string singleError)
        {
            return new[] { singleError };
        }

        return errors.Cast<object?>()
            .Where(x => x is not null)
            .Select(x => x!.ToString()!)
            .ToArray();
    }
}
