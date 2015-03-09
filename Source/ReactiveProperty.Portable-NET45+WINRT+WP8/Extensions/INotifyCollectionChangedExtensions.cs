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

namespace Reactive.Bindings.Extensions
{
    public static class INotifyCollectionChangedExtensions
    {
        /// <summary>Converts CollectionChanged to an observable sequence.</summary>
        public static IObservable<NotifyCollectionChangedEventArgs> CollectionChangedAsObservable<T>(this T source)
            where T : INotifyCollectionChanged
        {
            return Observable.FromEvent<NotifyCollectionChangedEventHandler, NotifyCollectionChangedEventArgs>(
                h => (sender, e) => h(e),
                h => source.CollectionChanged += h,
                h => source.CollectionChanged -= h);
        }

        /// <summary>Observe CollectionChanged:Add and take single item.</summary>
        public static IObservable<T> ObserveAddChanged<T>(this INotifyCollectionChanged source)
        {
            var result = source.CollectionChangedAsObservable()
                .Where(e => e.Action == NotifyCollectionChangedAction.Add)
                .Select(e => (T)e.NewItems[0]);
            return result;
        }

        /// <summary>Observe CollectionChanged:Add.</summary>
        public static IObservable<T[]> ObserveAddChangedItems<T>(this INotifyCollectionChanged source)
        {
            var result = source.CollectionChangedAsObservable()
                .Where(e => e.Action == NotifyCollectionChangedAction.Add)
                .Select(e => e.NewItems.Cast<T>().ToArray());
            return result;
        }

        /// <summary>Observe CollectionChanged:Remove and take single item.</summary>
        public static IObservable<T> ObserveRemoveChanged<T>(this INotifyCollectionChanged source)
        {
            var result = source.CollectionChangedAsObservable()
                .Where(e => e.Action == NotifyCollectionChangedAction.Remove)
                .Select(e => (T)e.OldItems[0]);
            return result;
        }

        /// <summary>Observe CollectionChanged:Remove.</summary>
        public static IObservable<T[]> ObserveRemoveChangedItems<T>(this INotifyCollectionChanged source)
        {
            var result = source.CollectionChangedAsObservable()
                .Where(e => e.Action == NotifyCollectionChangedAction.Remove)
                .Select(e => e.OldItems.Cast<T>().ToArray());
            return result;
        }

        /// <summary>Observe CollectionChanged:Move and take single item.</summary>
        public static IObservable<OldNewPair<T>> ObserveMoveChanged<T>(this INotifyCollectionChanged source)
        {
            var result = source.CollectionChangedAsObservable()
                .Where(e => e.Action == NotifyCollectionChangedAction.Move)
                .Select(e => new OldNewPair<T>((T)e.OldItems[0], (T)e.NewItems[0]));
            return result;
        }

        /// <summary>Observe CollectionChanged:Move.</summary>
        public static IObservable<OldNewPair<T[]>> ObserveMoveChangedItems<T>(this INotifyCollectionChanged source)
        {
            var result = source.CollectionChangedAsObservable()
                .Where(e => e.Action == NotifyCollectionChangedAction.Move)
                .Select(e => new OldNewPair<T[]>(e.OldItems.Cast<T>().ToArray(), e.NewItems.Cast<T>().ToArray()));
            return result;
        }

        /// <summary>Observe CollectionChanged:Replace and take single item.</summary>
        public static IObservable<OldNewPair<T>> ObserveReplaceChanged<T>(this INotifyCollectionChanged source)
        {
            var result = source.CollectionChangedAsObservable()
                .Where(e => e.Action == NotifyCollectionChangedAction.Replace)
                .Select(e => new OldNewPair<T>((T)e.OldItems[0], (T)e.NewItems[0]));
            return result;
        }

        /// <summary>Observe CollectionChanged:Replace.</summary>
        public static IObservable<OldNewPair<T[]>> ObserveReplaceChangedItems<T>(this INotifyCollectionChanged source)
        {
            var result = source.CollectionChangedAsObservable()
                .Where(e => e.Action == NotifyCollectionChangedAction.Replace)
                .Select(e => new OldNewPair<T[]>(e.OldItems.Cast<T>().ToArray(), e.NewItems.Cast<T>().ToArray()));
            return result;
        }

        /// <summary>Observe CollectionChanged:Reset.</summary>
        public static IObservable<Unit> ObserveResetChanged<T>(this INotifyCollectionChanged source)
        {
            var result = source.CollectionChangedAsObservable()
                .Where(e => e.Action == NotifyCollectionChangedAction.Reset)
                .Select(_ => new Unit());
            return result;
        }

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
        public static IObservable<PropertyPack<TElement, TProperty>> ObserveElementProperty<TCollection, TElement, TProperty>(this TCollection source, Expression<Func<TElement, TProperty>> propertySelector, bool isPushCurrentValueAtFirst = true)
            where TCollection : INotifyCollectionChanged, IEnumerable<TElement>
            where TElement : class, INotifyPropertyChanged
        {
            if (source == null)             throw new ArgumentNullException("source");
            if (propertySelector == null)   throw new ArgumentNullException("propertySelector");

            var memberExpression = (MemberExpression)propertySelector.Body;
            var propertyInfo = memberExpression.Member as PropertyInfo;
            if (propertyInfo == null)
                throw new ArgumentException("propertySelector is not property expression");

            return Observable.Create<PropertyPack<TElement, TProperty>>(observer =>
            {
                //--- cache element property subscriptions
                var subscriptionCache = new Dictionary<TElement, IDisposable>();

                //--- subscribe / unsubscribe property which all elements have
                Action<IEnumerable<TElement>> subscribe = elements =>
                {
                    foreach (var x in elements)
                    {
                        var subsctiption = x.ObserveProperty(propertySelector, isPushCurrentValueAtFirst)
                                         .Subscribe(y =>
                                         {
                                             var pair = PropertyPack.Create(x, propertyInfo, y);
                                             observer.OnNext(pair);
                                         });
                        subscriptionCache.Add(x, subsctiption);
                    }
                };
                Action unsubscribeAll = () =>
                {
                    foreach (var x in subscriptionCache.Values)
                        x.Dispose();
                    subscriptionCache.Clear();
                };
                subscribe(source);

                //--- hook collection changed
                var disposable = source.CollectionChangedAsObservable().Subscribe(x =>
                {
                    if (x.Action == NotifyCollectionChangedAction.Remove
                    ||  x.Action == NotifyCollectionChangedAction.Replace)
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
                    ||  x.Action == NotifyCollectionChangedAction.Replace)
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