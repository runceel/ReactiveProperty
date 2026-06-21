using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using R3;
using Reactive.Bindings.R3;

namespace Reactive.Bindings.R3.Extensions;

/// <summary>
/// Provides R3 observable helpers for <see cref="INotifyCollectionChanged"/>.
/// </summary>
public static class INotifyCollectionChangedExtensions
{
    /// <summary>
    /// Converts <see cref="INotifyCollectionChanged.CollectionChanged"/> to an observable sequence.
    /// </summary>
    /// <typeparam name="T">Source type.</typeparam>
    /// <param name="source">Source collection.</param>
    /// <returns>Collection changed event sequence.</returns>
    public static Observable<NotifyCollectionChangedEventArgs> CollectionChangedAsObservable<T>(this T source)
        where T : INotifyCollectionChanged
    {
        if (source is null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        return Observable.FromEvent<NotifyCollectionChangedEventHandler, NotifyCollectionChangedEventArgs>(
            h => (_, e) => h(e),
            h => source.CollectionChanged += h,
            h => source.CollectionChanged -= h,
            default);
    }

    /// <summary>
    /// Observes added items and takes the first item from each add event.
    /// </summary>
    public static Observable<T> ObserveAddChanged<T>(this INotifyCollectionChanged source) =>
        source.ObserveCollectionChangedItems<T>(NotifyCollectionChangedAction.Add, static e => e.NewItems!)
            .Select(static x => x[0]);

    /// <summary>
    /// Observes added item arrays.
    /// </summary>
    public static Observable<T[]> ObserveAddChangedItems<T>(this INotifyCollectionChanged source) =>
        source.ObserveCollectionChangedItems<T>(NotifyCollectionChangedAction.Add, static e => e.NewItems!);

    /// <summary>
    /// Observes removed items and takes the first item from each remove event.
    /// </summary>
    public static Observable<T> ObserveRemoveChanged<T>(this INotifyCollectionChanged source) =>
        source.ObserveCollectionChangedItems<T>(NotifyCollectionChangedAction.Remove, static e => e.OldItems!)
            .Select(static x => x[0]);

    /// <summary>
    /// Observes removed item arrays.
    /// </summary>
    public static Observable<T[]> ObserveRemoveChangedItems<T>(this INotifyCollectionChanged source) =>
        source.ObserveCollectionChangedItems<T>(NotifyCollectionChangedAction.Remove, static e => e.OldItems!);

    /// <summary>
    /// Observes moved items and takes the first item from each move event.
    /// </summary>
    public static Observable<OldNewPair<T>> ObserveMoveChanged<T>(this INotifyCollectionChanged source) =>
        source.ObserveMoveChangedItems<T>()
            .Select(static x => new OldNewPair<T>(x.OldItem[0], x.NewItem[0]));

    /// <summary>
    /// Observes moved item arrays.
    /// </summary>
    public static Observable<OldNewPair<T[]>> ObserveMoveChangedItems<T>(this INotifyCollectionChanged source) =>
        source.CollectionChangedAsObservable()
            .Where(static e => e.Action == NotifyCollectionChangedAction.Move)
            .Select(static e => new OldNewPair<T[]>([.. e.OldItems!.Cast<T>()], [.. e.NewItems!.Cast<T>()]));

    /// <summary>
    /// Observes replaced items and takes the first item from each replace event.
    /// </summary>
    public static Observable<OldNewPair<T>> ObserveReplaceChanged<T>(this INotifyCollectionChanged source) =>
        source.ObserveReplaceChangedItems<T>()
            .Select(static x => new OldNewPair<T>(x.OldItem[0], x.NewItem[0]));

    /// <summary>
    /// Observes replaced item arrays.
    /// </summary>
    public static Observable<OldNewPair<T[]>> ObserveReplaceChangedItems<T>(this INotifyCollectionChanged source) =>
        source.CollectionChangedAsObservable()
            .Where(static e => e.Action == NotifyCollectionChangedAction.Replace)
            .Select(static e => new OldNewPair<T[]>([.. e.OldItems!.Cast<T>()], [.. e.NewItems!.Cast<T>()]));

    /// <summary>
    /// Observes reset events.
    /// </summary>
    public static Observable<Unit> ObserveResetChanged<T>(this INotifyCollectionChanged source) =>
        source.CollectionChangedAsObservable()
            .Where(static e => e.Action == NotifyCollectionChangedAction.Reset)
            .Select(static _ => Unit.Default);

    /// <summary>
    /// Observes an element property in an <see cref="ObservableCollection{T}"/>.
    /// </summary>
    public static Observable<PropertyPack<TElement, TProperty>> ObserveElementProperty<TElement, TProperty>(
        this ObservableCollection<TElement> source,
        Func<TElement, TProperty> propertySelector,
        bool isPushCurrentValueAtFirst = true,
        [CallerArgumentExpression(nameof(propertySelector))] string? propertyName = null)
        where TElement : class, INotifyPropertyChanged =>
        ObserveElementPropertyCore(source, propertySelector, isPushCurrentValueAtFirst, propertyName);

    /// <summary>
    /// Observes an element property in a <see cref="ReadOnlyObservableCollection{T}"/>.
    /// </summary>
    public static Observable<PropertyPack<TElement, TProperty>> ObserveElementProperty<TElement, TProperty>(
        this ReadOnlyObservableCollection<TElement> source,
        Func<TElement, TProperty> propertySelector,
        bool isPushCurrentValueAtFirst = true,
        [CallerArgumentExpression(nameof(propertySelector))] string? propertyName = null)
        where TElement : class, INotifyPropertyChanged =>
        ObserveElementPropertyCore(source, propertySelector, isPushCurrentValueAtFirst, propertyName);

    /// <summary>
    /// Observes an observable property of each element in an <see cref="ObservableCollection{T}"/>.
    /// </summary>
    public static Observable<PropertyPack<TElement, TProperty>> ObserveElementObservableProperty<TElement, TProperty>(
        this ObservableCollection<TElement> source,
        Func<TElement, Observable<TProperty>> propertySelector,
        [CallerArgumentExpression(nameof(propertySelector))] string? propertyName = null)
        where TElement : class =>
        ObserveElementObservablePropertyCore(source, propertySelector, propertyName);

    /// <summary>
    /// Observes an observable property of each element in a <see cref="ReadOnlyObservableCollection{T}"/>.
    /// </summary>
    public static Observable<PropertyPack<TElement, TProperty>> ObserveElementObservableProperty<TElement, TProperty>(
        this ReadOnlyObservableCollection<TElement> source,
        Func<TElement, Observable<TProperty>> propertySelector,
        [CallerArgumentExpression(nameof(propertySelector))] string? propertyName = null)
        where TElement : class =>
        ObserveElementObservablePropertyCore(source, propertySelector, propertyName);

    /// <summary>
    /// Observes <see cref="INotifyPropertyChanged.PropertyChanged"/> of each element in an <see cref="ObservableCollection{T}"/>.
    /// </summary>
    public static Observable<SenderEventArgsPair<TElement, PropertyChangedEventArgs>> ObserveElementPropertyChanged<TElement>(
        this ObservableCollection<TElement> source)
        where TElement : class, INotifyPropertyChanged =>
        ObserveElementPropertyChangedCore(source);

    /// <summary>
    /// Observes <see cref="INotifyPropertyChanged.PropertyChanged"/> of each element in a <see cref="ReadOnlyObservableCollection{T}"/>.
    /// </summary>
    public static Observable<SenderEventArgsPair<TElement, PropertyChangedEventArgs>> ObserveElementPropertyChanged<TElement>(
        this ReadOnlyObservableCollection<TElement> source)
        where TElement : class, INotifyPropertyChanged =>
        ObserveElementPropertyChangedCore(source);

    /// <summary>
    /// Observes an element property in a filtered collection.
    /// </summary>
    public static Observable<PropertyPack<TElement, TProperty>> ObserveElementProperty<TElement, TProperty>(
        this IFilteredReadOnlyObservableCollection<TElement> source,
        Func<TElement, TProperty> propertySelector,
        bool isPushCurrentValueAtFirst = true,
        [CallerArgumentExpression(nameof(propertySelector))] string? propertyName = null)
        where TElement : class, INotifyPropertyChanged =>
        ObserveElementPropertyCore(source, propertySelector, isPushCurrentValueAtFirst, propertyName);

    /// <summary>
    /// Observes an observable property of each element in a filtered collection.
    /// </summary>
    public static Observable<PropertyPack<TElement, TProperty>> ObserveElementObservableProperty<TElement, TProperty>(
        this IFilteredReadOnlyObservableCollection<TElement> source,
        Func<TElement, Observable<TProperty>> propertySelector,
        [CallerArgumentExpression(nameof(propertySelector))] string? propertyName = null)
        where TElement : class, INotifyPropertyChanged =>
        ObserveElementObservablePropertyCore(source, propertySelector, propertyName);

    /// <summary>
    /// Observes <see cref="INotifyPropertyChanged.PropertyChanged"/> of each element in a filtered collection.
    /// </summary>
    public static Observable<SenderEventArgsPair<TElement, PropertyChangedEventArgs>> ObserveElementPropertyChanged<TElement>(
        this IFilteredReadOnlyObservableCollection<TElement> source)
        where TElement : class, INotifyPropertyChanged =>
        ObserveElementPropertyChangedCore(source);

    private static Observable<T[]> ObserveCollectionChangedItems<T>(
        this INotifyCollectionChanged source,
        NotifyCollectionChangedAction action,
        Func<NotifyCollectionChangedEventArgs, System.Collections.IList> itemSelector) =>
        source.CollectionChangedAsObservable()
            .Where(e => e.Action == action)
            .Select(e => (T[])[.. itemSelector(e).Cast<T>()]);

    internal static Observable<TResult> ObserveElementCore<TElement, TResult>(
        this IEnumerable<TElement> source,
        Func<TElement, Observer<TResult>, IDisposable> subscribeAction)
        where TElement : class
    {
        if (source is null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        return Observable.Create<TResult>(observer =>
        {
            var subscriptionCache = new Dictionary<TElement, IDisposable>();

            void Subscribe(IEnumerable<TElement> elements)
            {
                foreach (var element in elements)
                {
                    subscriptionCache.Add(element, subscribeAction(element, observer));
                }
            }

            void Unsubscribe(IEnumerable<TElement> elements)
            {
                foreach (var element in elements)
                {
                    if (subscriptionCache.TryGetValue(element, out var subscription))
                    {
                        subscription.Dispose();
                        subscriptionCache.Remove(element);
                    }
                }
            }

            void UnsubscribeAll()
            {
                IDisposable[] subscriptions = [.. subscriptionCache.Values];
                foreach (var subscription in subscriptions)
                {
                    subscription.Dispose();
                }

                subscriptionCache.Clear();
            }

            Subscribe(source);
            IDisposable? collectionSubscription = null;
            if (source is INotifyCollectionChanged collectionChanged)
            {
                collectionSubscription = collectionChanged.CollectionChangedAsObservable().Subscribe(e =>
                {
                    if (e.Action is NotifyCollectionChangedAction.Remove or NotifyCollectionChangedAction.Replace)
                    {
                        Unsubscribe(e.OldItems!.Cast<TElement>());
                    }

                    if (e.Action is NotifyCollectionChangedAction.Add or NotifyCollectionChangedAction.Replace)
                    {
                        Subscribe(e.NewItems!.Cast<TElement>());
                    }

                    if (e.Action == NotifyCollectionChangedAction.Reset)
                    {
                        UnsubscribeAll();
                        Subscribe(source);
                    }
                });
            }

            return Disposable.Create(() =>
            {
                collectionSubscription?.Dispose();
                UnsubscribeAll();
            });
        });
    }

    private static Observable<PropertyPack<TElement, TProperty>> ObserveElementPropertyCore<TElement, TProperty>(
        IEnumerable<TElement> source,
        Func<TElement, TProperty> propertySelector,
        bool isPushCurrentValueAtFirst,
        string? propertyName)
        where TElement : class, INotifyPropertyChanged
    {
        if (propertySelector is null)
        {
            throw new ArgumentNullException(nameof(propertySelector));
        }

        var resolvedPropertyName = ResolvePropertyName(propertyName);
        var propertyInfo = GetPropertyInfo<TElement>(resolvedPropertyName);
        return source.ObserveElementCore<TElement, PropertyPack<TElement, TProperty>>((element, observer) =>
            Observable.ObservePropertyChanged(element, propertySelector, isPushCurrentValueAtFirst, default, resolvedPropertyName)
                .Subscribe(value => observer.OnNext(new PropertyPack<TElement, TProperty>(element, propertyInfo, value))));
    }

    private static Observable<PropertyPack<TElement, TProperty>> ObserveElementObservablePropertyCore<TElement, TProperty>(
        IEnumerable<TElement> source,
        Func<TElement, Observable<TProperty>> propertySelector,
        string? propertyName)
        where TElement : class
    {
        if (propertySelector is null)
        {
            throw new ArgumentNullException(nameof(propertySelector));
        }

        var resolvedPropertyName = ResolvePropertyName(propertyName);
        var propertyInfo = GetPropertyInfo<TElement>(resolvedPropertyName);
        return source.ObserveElementCore<TElement, PropertyPack<TElement, TProperty>>((element, observer) =>
            propertySelector(element)
                .Subscribe(value => observer.OnNext(new PropertyPack<TElement, TProperty>(element, propertyInfo, value))));
    }

    private static Observable<SenderEventArgsPair<TElement, PropertyChangedEventArgs>> ObserveElementPropertyChangedCore<TElement>(
        IEnumerable<TElement> source)
        where TElement : class, INotifyPropertyChanged =>
        source.ObserveElementCore<TElement, SenderEventArgsPair<TElement, PropertyChangedEventArgs>>((element, observer) =>
            Observable.FromEvent<PropertyChangedEventHandler, PropertyChangedEventArgs>(
                    h => (_, e) => h(e),
                    h => element.PropertyChanged += h,
                    h => element.PropertyChanged -= h,
                    default)
                .Subscribe(args => observer.OnNext(new SenderEventArgsPair<TElement, PropertyChangedEventArgs>(element, args))));

    private static PropertyInfo GetPropertyInfo<TElement>(string propertyName) =>
        typeof(TElement).GetProperty(propertyName)
        ?? throw new ArgumentException($"'{propertyName}' is not a property of '{typeof(TElement).FullName}'.", nameof(propertyName));

    private static string ResolvePropertyName(string? propertyExpression)
    {
        if (string.IsNullOrWhiteSpace(propertyExpression))
        {
            throw new ArgumentException("Property selector is required.", "propertyName");
        }

        var expression = propertyExpression!.Trim();

        // Strip an optional lambda prefix ("x =>", "(x) =>", "static x =>") so only the body remains.
        var arrowIndex = expression.IndexOf("=>", StringComparison.Ordinal);
        if (arrowIndex < 0)
        {
            // An explicit, already-resolved property name (e.g. propertyName: nameof(Item.Name)).
            var bare = StripVerbatimPrefix(expression);
            if (IsValidIdentifier(bare))
            {
                return bare;
            }

            throw NotSingleLevelSelector(propertyExpression);
        }

        var body = expression.Substring(arrowIndex + 2).Trim();

        // A single-level selector body is "<receiver>.<Property>" with exactly one top-level member
        // access. Dots inside parentheses/brackets (e.g. a cast such as "((My.Ns.Base)x).Name") do not
        // count, so casts are accepted while nested paths ("x.Child.Name") are rejected.
        if (!TryGetSingleLevelProperty(body, out var name))
        {
            throw NotSingleLevelSelector(propertyExpression);
        }

        return name;
    }

    private static bool TryGetSingleLevelProperty(string body, out string name)
    {
        name = string.Empty;
        var depth = 0;
        var topLevelDotCount = 0;
        var lastTopLevelDot = -1;
        for (var i = 0; i < body.Length; i++)
        {
            switch (body[i])
            {
                case '(':
                case '[':
                    depth++;
                    break;
                case ')':
                case ']':
                    depth--;
                    break;
                case '.' when depth == 0:
                    topLevelDotCount++;
                    lastTopLevelDot = i;
                    break;
            }
        }

        if (topLevelDotCount != 1)
        {
            return false;
        }

        var candidate = StripVerbatimPrefix(body.Substring(lastTopLevelDot + 1).Trim());
        if (!IsValidIdentifier(candidate))
        {
            return false;
        }

        name = candidate;
        return true;
    }

    private static string StripVerbatimPrefix(string value) =>
        value.StartsWith("@", StringComparison.Ordinal) ? value.Substring(1) : value;

    private static ArgumentException NotSingleLevelSelector(string propertyExpression) =>
        new(
            $"Only a single-level property selector such as 'x => x.PropertyName' is supported, but '{propertyExpression}' was provided. Nested property paths are not supported.",
            "propertyName");

    private static bool IsValidIdentifier(string value)
    {
        if (value.Length == 0 || !(char.IsLetter(value[0]) || value[0] == '_'))
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
}
