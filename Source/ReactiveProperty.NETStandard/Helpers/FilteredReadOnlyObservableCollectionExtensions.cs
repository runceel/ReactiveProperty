using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Text;
using Reactive.Bindings.Extensions;

namespace Reactive.Bindings.Helpers
{
    /// <summary>
    /// IFilteredReadOnlyObservableCollection extensions
    /// </summary>
    public static class FilteredReadOnlyObservableCollectionExtensions
    {
        /// <summary>
        /// Observe collection element's property.
        /// </summary>
        /// <typeparam name="TElement">Type of element</typeparam>
        /// <typeparam name="TProperty">Type of property</typeparam>
        /// <param name="source">Data source</param>
        /// <param name="propertySelector">Property selection expression</param>
        /// <param name="isPushCurrentValueAtFirst">Push current value on first subscribe</param>
        /// <returns>Property sequence</returns>
        public static IObservable<PropertyPack<TElement, TProperty>> ObserveElementProperty<TElement, TProperty>(this IFilteredReadOnlyObservableCollection<TElement> source, Expression<Func<TElement, TProperty>> propertySelector, bool isPushCurrentValueAtFirst = true)
            where TElement : class, INotifyPropertyChanged =>
            CollectionUtilities.ObserveElementProperty(source, propertySelector, isPushCurrentValueAtFirst);

        /// <summary>
        /// Observe collection element's IObservable sequence.
        /// </summary>
        /// <typeparam name="TElement">Collection element type</typeparam>
        /// <typeparam name="TProperty">Type of observable property element</typeparam>
        /// <param name="source">Source collection</param>
        /// <param name="propertySelector">IObservable selection expression</param>
        /// <returns>IObservable sequence sequence</returns>
        public static IObservable<PropertyPack<TElement, TProperty>> ObserveElementObservableProperty<TElement, TProperty>(this IFilteredReadOnlyObservableCollection<TElement> source, Expression<Func<TElement, IObservable<TProperty>>> propertySelector)
            where TElement : class, INotifyPropertyChanged =>
            CollectionUtilities.ObserveElementObservableProperty(source, propertySelector);

        /// <summary>
        /// Observe collection element's PropertyChanged event.
        /// </summary>
        /// <typeparam name="TElement">Type of Element</typeparam>
        /// <param name="source">source collection</param>
        /// <returns>PropertyChanged event stream.</returns>
        public static IObservable<SenderEventArgsPair<TElement, PropertyChangedEventArgs>> ObserveElementPropertyChanged<TElement>(this IFilteredReadOnlyObservableCollection<TElement> source)
            where TElement : class, INotifyPropertyChanged =>
            CollectionUtilities.ObserveElementPropertyChanged<IFilteredReadOnlyObservableCollection<TElement>, TElement>(source);

    }
}
