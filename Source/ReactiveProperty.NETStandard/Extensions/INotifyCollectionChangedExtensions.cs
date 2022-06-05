using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reflection;
using Reactive.Bindings.Internals;

namespace Reactive.Bindings.Extensions;

// TODO: remove this line later.
#nullable disable

/// <summary>
/// INotify Collection Changed Extensions
/// </summary>
public static class INotifyCollectionChangedExtensions
{
    /// <summary>
    /// Converts CollectionChanged to an observable sequence.
    /// </summary>
    public static IObservable<NotifyCollectionChangedEventArgs> CollectionChangedAsObservable<T>(this T source)
        where T : INotifyCollectionChanged =>
        InternalObservable.FromEvent<NotifyCollectionChangedEventHandler, NotifyCollectionChangedEventArgs>(
            h => (sender, e) => h(e),
            h => source.CollectionChanged += h,
            h => source.CollectionChanged -= h);

    /// <summary>
    /// Observe CollectionChanged:Add and take single item.
    /// </summary>
    public static IObservable<T> ObserveAddChanged<T>(this INotifyCollectionChanged source) =>
        source.CollectionChangedAsObservable()
            .Where(e => e.Action == NotifyCollectionChangedAction.Add)
            .Select(e => (T)e.NewItems[0]);

    /// <summary>
    /// Observe CollectionChanged:Add.
    /// </summary>
    public static IObservable<T[]> ObserveAddChangedItems<T>(this INotifyCollectionChanged source) =>
        source.CollectionChangedAsObservable()
            .Where(e => e.Action == NotifyCollectionChangedAction.Add)
            .Select(e => e.NewItems.Cast<T>().ToArray());

    /// <summary>
    /// Observe CollectionChanged:Remove and take single item.
    /// </summary>
    public static IObservable<T> ObserveRemoveChanged<T>(this INotifyCollectionChanged source) =>
        source.CollectionChangedAsObservable()
            .Where(e => e.Action == NotifyCollectionChangedAction.Remove)
            .Select(e => (T)e.OldItems[0]!);

    /// <summary>
    /// Observe CollectionChanged:Remove.
    /// </summary>
    public static IObservable<T[]> ObserveRemoveChangedItems<T>(this INotifyCollectionChanged source) =>
        source.CollectionChangedAsObservable()
            .Where(e => e.Action == NotifyCollectionChangedAction.Remove)
            .Select(e => e.OldItems.Cast<T>().ToArray());

    /// <summary>
    /// Observe CollectionChanged:Move and take single item.
    /// </summary>
    public static IObservable<OldNewPair<T>> ObserveMoveChanged<T>(this INotifyCollectionChanged source) =>
        source.CollectionChangedAsObservable()
            .Where(e => e.Action == NotifyCollectionChangedAction.Move)
            .Select(e => new OldNewPair<T>((T)e.OldItems[0], (T)e.NewItems[0]));

    /// <summary>
    /// Observe CollectionChanged:Move.
    /// </summary>
    public static IObservable<OldNewPair<T[]>> ObserveMoveChangedItems<T>(this INotifyCollectionChanged source) =>
        source.CollectionChangedAsObservable()
            .Where(e => e.Action == NotifyCollectionChangedAction.Move)
            .Select(e => new OldNewPair<T[]>(e.OldItems.Cast<T>().ToArray(), e.NewItems.Cast<T>().ToArray()));

    /// <summary>
    /// Observe CollectionChanged:Replace and take single item.
    /// </summary>
    public static IObservable<OldNewPair<T>> ObserveReplaceChanged<T>(this INotifyCollectionChanged source) =>
        source.CollectionChangedAsObservable()
            .Where(e => e.Action == NotifyCollectionChangedAction.Replace)
            .Select(e => new OldNewPair<T>((T)e.OldItems[0], (T)e.NewItems[0]));

    /// <summary>
    /// Observe CollectionChanged:Replace.
    /// </summary>
    public static IObservable<OldNewPair<T[]>> ObserveReplaceChangedItems<T>(this INotifyCollectionChanged source) =>
        source.CollectionChangedAsObservable()
            .Where(e => e.Action == NotifyCollectionChangedAction.Replace)
            .Select(e => new OldNewPair<T[]>(e.OldItems.Cast<T>().ToArray(), e.NewItems.Cast<T>().ToArray()));

    /// <summary>
    /// Observe CollectionChanged:Reset.
    /// </summary>
    public static IObservable<Unit> ObserveResetChanged<T>(this INotifyCollectionChanged source) =>
        source.CollectionChangedAsObservable()
            .Where(e => e.Action == NotifyCollectionChangedAction.Reset)
            .Select(_ => new Unit());

}
