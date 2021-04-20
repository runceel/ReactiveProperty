using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reflection;
using Reactive.Bindings.Extensions;
using Reactive.Bindings.Internals;

namespace Reactive.Bindings.Helpers
{
    /// <summary>
    /// Collection utilities for creating ObserveElementProperty, ObserveElementObservableProperty and ObserveElementPropertyChanged extension methods for custom collection classes.
    /// If you would like to use those methods on ObservableCollection, then please use pre defined extension methods for ObservableCollection.
    /// </summary>
    public static class CollectionUtilities
    {
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
        public static IObservable<PropertyPack<TElement, TProperty>> ObserveElementProperty<TCollection, TElement, TProperty>(TCollection source, Expression<Func<TElement, TProperty>> propertySelector, bool isPushCurrentValueAtFirst = true)
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

            return ObserveElementCore<TCollection, TElement, PropertyPack<TElement, TProperty>>
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
        public static IObservable<PropertyPack<TElement, TProperty>> ObserveElementObservableProperty<TCollection, TElement, TProperty>(TCollection source, Expression<Func<TElement, IObservable<TProperty>>> propertySelector)
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

            return ObserveElementCore<TCollection, TElement, PropertyPack<TElement, TProperty>>
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
        public static IObservable<SenderEventArgsPair<TElement, PropertyChangedEventArgs>> ObserveElementPropertyChanged<TCollection, TElement>(TCollection source)
            where TCollection : INotifyCollectionChanged, IEnumerable<TElement>
            where TElement : class, INotifyPropertyChanged
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            return ObserveElementCore<TCollection, TElement, SenderEventArgsPair<TElement, PropertyChangedEventArgs>>
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
