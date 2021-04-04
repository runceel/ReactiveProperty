﻿using System;
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
using This = Reactive.Bindings.Extensions.INotifyCollectionChangedExtensions;

namespace Reactive.Bindings.Extensions
{
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
        public static IObservable<T?> ObserveAddChanged<T>(this INotifyCollectionChanged source) =>
            source.CollectionChangedAsObservable()
                .Where(e => e.Action == NotifyCollectionChangedAction.Add)
                .Select(e => (T?)e.NewItems[0]);

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
                .Select(e => (T)e.OldItems[0]);

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

        /// <summary>
        /// Observe collection element's property.
        /// </summary>
        /// <typeparam name="TCollection">Type of collection</typeparam>
        /// <typeparam name="TElement">Type of element</typeparam>
        /// <typeparam name="TProperty">Type of property</typeparam>
        /// <param name="source">Data source</param>
        /// <param name="propertySelector">Property selection expression</param>
        /// <param name="isPushCurrentValueAtFirst">Push current value on first subscribe</param>
        /// <returns>Property value sequence</returns>
        internal static IObservable<PropertyPack<TElement, TProperty>> ObserveElementProperty<TCollection, TElement, TProperty>(this TCollection source, Expression<Func<TElement, TProperty>> propertySelector, bool isPushCurrentValueAtFirst = true)
            where TCollection : INotifyCollectionChanged, IEnumerable<TElement>
            where TElement : class, INotifyPropertyChanged
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (propertySelector == null)
            {
                throw new ArgumentNullException(nameof(propertySelector));
            }

            var memberExpression = (MemberExpression)propertySelector.Body;
            var propertyInfo = memberExpression.Member as PropertyInfo;
            if (propertyInfo == null)
            {
                throw new ArgumentException($"{nameof(propertySelector)} is not property expression");
            }

            return This.ObserveElementCore<TCollection, TElement, PropertyPack<TElement, TProperty>>
            (
                source,
                (x, observer) => x.ObserveProperty(propertySelector, isPushCurrentValueAtFirst).Subscribe(y =>
                {
                    var pair = PropertyPack.Create(x, propertyInfo, y);
                    observer.OnNext(pair);
                })
            );
        }

        /// <summary>
        /// Observe collection element's IObservable sequence.
        /// </summary>
        /// <typeparam name="TCollection">Type of collection</typeparam>
        /// <typeparam name="TElement">Type of collection element</typeparam>
        /// <typeparam name="TProperty">Type of observable property element</typeparam>
        /// <param name="source">Source collection</param>
        /// <param name="propertySelector">IObservable selection expression</param>
        /// <returns>IObservable sequence</returns>
        internal static IObservable<PropertyPack<TElement, TProperty>> ObserveElementObservableProperty<TCollection, TElement, TProperty>(this TCollection source, Expression<Func<TElement, IObservable<TProperty>>> propertySelector)
            where TCollection : INotifyCollectionChanged, IEnumerable<TElement>
            where TElement : class
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (propertySelector == null)
            {
                throw new ArgumentNullException(nameof(propertySelector));
            }

            if (!(propertySelector.Body is MemberExpression memberExpression))
            {
                if (!(propertySelector.Body is UnaryExpression unaryExpression)) { throw new ArgumentException(nameof(propertySelector)); }
                memberExpression = unaryExpression.Operand as MemberExpression;
                if (memberExpression == null) { throw new ArgumentException(nameof(propertySelector)); }
            }

            var propertyInfo = memberExpression.Member as PropertyInfo;
            if (propertyInfo == null)
            {
                throw new ArgumentException($"{nameof(propertySelector)} is not property expression");
            }

            // no use
            var getter = AccessorCache<TElement>.LookupGet(propertySelector, out var propertyName);

            return This.ObserveElementCore<TCollection, TElement, PropertyPack<TElement, TProperty>>
            (
                source,
                (x, observer) => getter(x).Subscribe(y =>
                {
                    var pair = PropertyPack.Create(x, propertyInfo, y);
                    observer.OnNext(pair);
                })
            );
        }

        /// <summary>
        /// Observe collection element's PropertyChanged event.
        /// </summary>
        /// <typeparam name="TCollection">Type of collection</typeparam>
        /// <typeparam name="TElement">Type of element</typeparam>
        /// <param name="source">The source.</param>
        /// <returns>PropertyChanged event stream.</returns>
        /// <exception cref="System.ArgumentNullException">source</exception>
        internal static IObservable<SenderEventArgsPair<TElement, PropertyChangedEventArgs>> ObserveElementPropertyChanged<TCollection, TElement>(this TCollection source)
            where TCollection : INotifyCollectionChanged, IEnumerable<TElement>
            where TElement : class, INotifyPropertyChanged
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            return This.ObserveElementCore<TCollection, TElement, SenderEventArgsPair<TElement, PropertyChangedEventArgs>>
            (
                source,
                (x, observer) => x.PropertyChangedAsObservable().Subscribe(y =>
                {
                    var pair = SenderEventArgsPair.Create(x, y);
                    observer.OnNext(pair);
                })
            );
        }


        /// <summary>
        /// Core logic of ObserveElementXXXXX methods.
        /// </summary>
        /// <typeparam name="TCollection">The type of the collection.</typeparam>
        /// <typeparam name="TElement">Type of element.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="source">source collection</param>
        /// <param name="subscribeAction">element subscribe logic.</param>
        /// <returns></returns>
        private static IObservable<TResult> ObserveElementCore<TCollection, TElement, TResult>(TCollection source, Func<TElement, IObserver<TResult>, IDisposable> subscribeAction)
            where TCollection : INotifyCollectionChanged, IEnumerable<TElement>
            where TElement : class
        {
            return Observable.Create<TResult>(observer =>
            {
                //--- cache element property subscriptions
                var subscriptionCache = new Dictionary<object, IDisposable>();

                //--- subscribe / unsubscribe property which all elements have
                void subscribe(IEnumerable<TElement> elements)
                {
                    foreach (var x in elements)
                    {
                        var subscription = subscribeAction(x, observer);
                        subscriptionCache.Add(x, subscription);
                    }
                }
                void unsubscribeAll()
                {
                    foreach (var x in subscriptionCache.Values)
                    {
                        x.Dispose();
                    }

                    subscriptionCache.Clear();
                }
                subscribe(source);

                //--- hook collection changed
                var disposable = source.CollectionChangedAsObservable().Subscribe(x =>
                {
                    if (x.Action == NotifyCollectionChangedAction.Remove
                    || x.Action == NotifyCollectionChangedAction.Replace)
                    {
                        //--- unsubscribe
                        var oldItems = x.OldItems.Cast<TElement>();
                        foreach (var y in oldItems)
                        {
                            subscriptionCache[y].Dispose();
                            subscriptionCache.Remove(y);
                        }
                    }

                    if (x.Action == NotifyCollectionChangedAction.Add
                    || x.Action == NotifyCollectionChangedAction.Replace)
                    {
                        var newItems = x.NewItems.Cast<TElement>();
                        subscribe(newItems);
                    }

                    if (x.Action == NotifyCollectionChangedAction.Reset)
                    {
                        unsubscribeAll();
                        subscribe(source);
                    }
                });

                //--- unsubscribe
                return Disposable.Create(() =>
                {
                    disposable.Dispose();
                    unsubscribeAll();
                });
            });
        }
    }
}
