using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using R3;

namespace Reactive.Bindings.R3.Extensions;

/// <summary>
/// Provides R3 helpers for <see cref="INotifyPropertyChanged"/>.
/// </summary>
public static class INotifyPropertyChangedExtensions
{
    /// <summary>
    /// Creates a two-way synchronized bindable reactive property.
    /// </summary>
    public static BindableReactiveProperty<TProperty> ToReactivePropertyAsSynchronized<TSubject, TProperty>(
        this TSubject subject,
        Func<TSubject, TProperty> propertySelector,
        bool pushCurrentValueOnSubscribe = true,
        [CallerArgumentExpression(nameof(propertySelector))] string? propertyName = null)
        where TSubject : INotifyPropertyChanged
    {
        if (subject is null)
        {
            throw new ArgumentNullException(nameof(subject));
        }

        if (propertySelector is null)
        {
            throw new ArgumentNullException(nameof(propertySelector));
        }

        var resolvedPropertyName = ResolvePropertyName<TSubject>(propertyName);
        var property = new SynchronizedBindableReactiveProperty<TProperty>(propertySelector(subject));
        property.Add(Observable.ObservePropertyChanged(subject, propertySelector, pushCurrentValueOnSubscribe, default, resolvedPropertyName)
            .Subscribe(value => property.Value = value));
        property.Add(property.Subscribe(value => SetProperty(subject, resolvedPropertyName, value)));
        return property;
    }

    /// <summary>
    /// Creates a two-way synchronized bindable reactive property with conversion.
    /// </summary>
    public static BindableReactiveProperty<TResult> ToReactivePropertyAsSynchronized<TSubject, TProperty, TResult>(
        this TSubject subject,
        Func<TSubject, TProperty> propertySelector,
        Func<TProperty, TResult> convert,
        Func<TResult, TProperty> convertBack,
        bool pushCurrentValueOnSubscribe = true,
        [CallerArgumentExpression(nameof(propertySelector))] string? propertyName = null)
        where TSubject : INotifyPropertyChanged
    {
        if (convert is null)
        {
            throw new ArgumentNullException(nameof(convert));
        }

        if (convertBack is null)
        {
            throw new ArgumentNullException(nameof(convertBack));
        }

        var resolvedPropertyName = ResolvePropertyName<TSubject>(propertyName);
        var property = new SynchronizedBindableReactiveProperty<TResult>(convert(propertySelector(subject)));
        property.Add(Observable.ObservePropertyChanged(subject, propertySelector, pushCurrentValueOnSubscribe, default, resolvedPropertyName)
            .Select(convert)
            .Subscribe(value => property.Value = value));
        property.Add(property.Subscribe(value => SetProperty(subject, resolvedPropertyName, convertBack(value))));
        return property;
    }

    /// <summary>
    /// Creates a two-way synchronized bindable reactive property with observable conversion.
    /// </summary>
    public static BindableReactiveProperty<TResult> ToReactivePropertyAsSynchronized<TSubject, TProperty, TResult>(
        this TSubject subject,
        Func<TSubject, TProperty> propertySelector,
        Func<Observable<TProperty>, Observable<TResult>> convert,
        Func<Observable<TResult>, Observable<TProperty>> convertBack,
        bool pushCurrentValueOnSubscribe = true,
        [CallerArgumentExpression(nameof(propertySelector))] string? propertyName = null)
        where TSubject : INotifyPropertyChanged
    {
        if (convert is null)
        {
            throw new ArgumentNullException(nameof(convert));
        }

        if (convertBack is null)
        {
            throw new ArgumentNullException(nameof(convertBack));
        }

        var resolvedPropertyName = ResolvePropertyName<TSubject>(propertyName);
        var initial = default(TResult)!;
        using (convert(Observable.Return(propertySelector(subject))).Subscribe(x => initial = x))
        {
        }

        var property = new SynchronizedBindableReactiveProperty<TResult>(initial);
        property.Add(convert(Observable.ObservePropertyChanged(subject, propertySelector, pushCurrentValueOnSubscribe, default, resolvedPropertyName))
            .Subscribe(value => property.Value = value));
        property.Add(convertBack(property).Subscribe(value => SetProperty(subject, resolvedPropertyName, value)));
        return property;
    }

    /// <summary>
    /// Creates a two-way synchronized bindable reactive property for a two-hop property path.
    /// </summary>
    public static BindableReactiveProperty<TProperty2> ToReactivePropertyAsSynchronized<TSubject, TProperty1, TProperty2>(
        this TSubject subject,
        Func<TSubject, TProperty1> propertySelector1,
        Func<TProperty1, TProperty2> propertySelector2,
        bool pushCurrentValueOnSubscribe = true,
        [CallerArgumentExpression(nameof(propertySelector1))] string? propertyName1 = null,
        [CallerArgumentExpression(nameof(propertySelector2))] string? propertyName2 = null)
        where TSubject : INotifyPropertyChanged
        where TProperty1 : class?, INotifyPropertyChanged
    {
        var resolvedPropertyName1 = ResolvePropertyName<TSubject>(propertyName1);
        var resolvedPropertyName2 = ResolvePropertyName<TProperty1>(propertyName2);
        var initialParent = propertySelector1(subject);
        var initial = initialParent is null ? default! : propertySelector2(initialParent);
        var property = new SynchronizedBindableReactiveProperty<TProperty2>(initial);
        property.Add(Observable.ObservePropertyChanged(subject, propertySelector1, propertySelector2, pushCurrentValueOnSubscribe, default, resolvedPropertyName1, resolvedPropertyName2)
            .Subscribe(value => property.Value = value));
        property.Add(property.Subscribe(value =>
        {
            var parent = propertySelector1(subject);
            if (parent is not null)
            {
                SetProperty(parent, resolvedPropertyName2, value);
            }
        }));
        return property;
    }

    /// <summary>
    /// Creates a two-way synchronized bindable reactive property for a three-hop property path.
    /// </summary>
    public static BindableReactiveProperty<TProperty3> ToReactivePropertyAsSynchronized<TSubject, TProperty1, TProperty2, TProperty3>(
        this TSubject subject,
        Func<TSubject, TProperty1> propertySelector1,
        Func<TProperty1, TProperty2> propertySelector2,
        Func<TProperty2, TProperty3> propertySelector3,
        bool pushCurrentValueOnSubscribe = true,
        [CallerArgumentExpression(nameof(propertySelector1))] string? propertyName1 = null,
        [CallerArgumentExpression(nameof(propertySelector2))] string? propertyName2 = null,
        [CallerArgumentExpression(nameof(propertySelector3))] string? propertyName3 = null)
        where TSubject : INotifyPropertyChanged
        where TProperty1 : class?, INotifyPropertyChanged
        where TProperty2 : class?, INotifyPropertyChanged
    {
        var resolvedPropertyName1 = ResolvePropertyName<TSubject>(propertyName1);
        var resolvedPropertyName2 = ResolvePropertyName<TProperty1>(propertyName2);
        var resolvedPropertyName3 = ResolvePropertyName<TProperty2>(propertyName3);
        var initialParent1 = propertySelector1(subject);
        var initialParent2 = initialParent1 is null ? null : propertySelector2(initialParent1);
        var initial = initialParent2 is null ? default! : propertySelector3(initialParent2);
        var property = new SynchronizedBindableReactiveProperty<TProperty3>(initial);
        property.Add(Observable.ObservePropertyChanged(subject, propertySelector1, propertySelector2, propertySelector3, pushCurrentValueOnSubscribe, default, resolvedPropertyName1, resolvedPropertyName2, resolvedPropertyName3)
            .Subscribe(value => property.Value = value));
        property.Add(property.Subscribe(value =>
        {
            var parent1 = propertySelector1(subject);
            var parent2 = parent1 is null ? null : propertySelector2(parent1);
            if (parent2 is not null)
            {
                SetProperty(parent2, resolvedPropertyName3, value);
            }
        }));
        return property;
    }

    /// <summary>
    /// Creates a two-way synchronized reactive property.
    /// </summary>
    public static ReactiveProperty<TProperty> ToReactivePropertySlimAsSynchronized<TSubject, TProperty>(
        this TSubject subject,
        Func<TSubject, TProperty> propertySelector,
        bool pushCurrentValueOnSubscribe = true,
        [CallerArgumentExpression(nameof(propertySelector))] string? propertyName = null)
        where TSubject : INotifyPropertyChanged
    {
        var resolvedPropertyName = ResolvePropertyName<TSubject>(propertyName);
        var property = new SynchronizedReactiveProperty<TProperty>(propertySelector(subject));
        property.Add(Observable.ObservePropertyChanged(subject, propertySelector, pushCurrentValueOnSubscribe, default, resolvedPropertyName)
            .Subscribe(value => property.Value = value));
        property.Add(property.Subscribe(value => SetProperty(subject, resolvedPropertyName, value)));
        return property;
    }

    private static void SetProperty<TTarget, TValue>(TTarget target, string propertyName, TValue value)
    {
        var propertyInfo = typeof(TTarget).GetProperty(propertyName)
            ?? throw new ArgumentException($"'{propertyName}' is not a property of '{typeof(TTarget).FullName}'.", nameof(propertyName));
        propertyInfo.SetValue(target, value, null);
    }

    private static string ResolvePropertyName<TTarget>(string? propertyExpression)
    {
        var propertyName = ParsePropertyName(propertyExpression);
        if (typeof(TTarget).GetProperty(propertyName) is null)
        {
            throw new ArgumentException($"'{propertyName}' is not a property of '{typeof(TTarget).FullName}'.", nameof(propertyExpression));
        }

        return propertyName;
    }

    private static string ParsePropertyName(string? propertyExpression)
    {
        if (string.IsNullOrWhiteSpace(propertyExpression))
        {
            throw new ArgumentException("Property selector is required.", nameof(propertyExpression));
        }

        var expression = propertyExpression!.Trim();
        var arrowIndex = expression.IndexOf("=>", StringComparison.Ordinal);
        var body = arrowIndex < 0 ? expression : expression.Substring(arrowIndex + 2).Trim();
        body = body.TrimEnd('!');

        if (body.IndexOfAny(new[] { '(', ')', '[', ']', '?', ':', ',', '+' }) >= 0)
        {
            throw new ArgumentException($"Unsupported property selector: '{propertyExpression}'.", nameof(propertyExpression));
        }

        var segments = body.Split('.', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
        if (segments.Length == 0 || segments.Length > 2)
        {
            throw new ArgumentException($"Unsupported property selector: '{propertyExpression}'.", nameof(propertyExpression));
        }

        var name = segments[^1];
        if (!IsValidIdentifier(name))
        {
            throw new ArgumentException($"Unsupported property selector: '{propertyExpression}'.", nameof(propertyExpression));
        }

        return name;
    }

    private static bool IsValidIdentifier(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return false;
        }

        if (!(char.IsLetter(value[0]) || value[0] == '_'))
        {
            return false;
        }

        for (var i = 1; i < value.Length; i++)
        {
            if (!(char.IsLetterOrDigit(value[i]) || value[i] == '_'))
            {
                return false;
            }
        }

        return true;
    }

    private sealed class SynchronizedBindableReactiveProperty<T> : BindableReactiveProperty<T>
    {
        private readonly List<IDisposable> _subscriptions = new();

        public SynchronizedBindableReactiveProperty(T value)
            : base(value)
        {
        }

        public void Add(IDisposable subscription) => _subscriptions.Add(subscription);

        protected override void DisposeCore()
        {
            foreach (var subscription in _subscriptions)
            {
                subscription.Dispose();
            }

            _subscriptions.Clear();
            base.DisposeCore();
        }
    }

    private sealed class SynchronizedReactiveProperty<T> : ReactiveProperty<T>
    {
        private readonly List<IDisposable> _subscriptions = new();

        public SynchronizedReactiveProperty(T value)
            : base(value)
        {
        }

        public void Add(IDisposable subscription) => _subscriptions.Add(subscription);

        protected override void DisposeCore()
        {
            foreach (var subscription in _subscriptions)
            {
                subscription.Dispose();
            }

            _subscriptions.Clear();
            base.DisposeCore();
        }
    }
}
