using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq.Expressions;
using Reactive.Bindings.Extensions;

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
            where TElement : class, INotifyPropertyChanged =>
            INotifyCollectionChangedExtensions.ObserveElementProperty(source, propertySelector, isPushCurrentValueAtFirst);

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
            where TElement : class =>
            INotifyCollectionChangedExtensions.ObserveElementObservableProperty(source, propertySelector);

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
            where TElement : class, INotifyPropertyChanged =>
            INotifyCollectionChangedExtensions.ObserveElementPropertyChanged<TCollection, TElement>(source);

    }
}
