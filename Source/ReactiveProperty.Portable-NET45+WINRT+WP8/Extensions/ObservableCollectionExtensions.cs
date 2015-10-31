using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reactive;

namespace Reactive.Bindings.Extensions
{
    public static class ObservableCollectionExtensions
    {
        #region ObservableCollection
        /// <summary>Observe CollectionChanged:Add and take single item.</summary>
        public static IObservable<T> ObserveAddChanged<T>(this ObservableCollection<T> source) =>
            ((INotifyCollectionChanged)source).ObserveAddChanged<T>();

        /// <summary>Observe CollectionChanged:Add.</summary>
        public static IObservable<T[]> ObserveAddChangedItems<T>(this ObservableCollection<T> source) =>
            ((INotifyCollectionChanged)source).ObserveAddChangedItems<T>();

        /// <summary>Observe CollectionChanged:Remove and take single item.</summary>
        public static IObservable<T> ObserveRemoveChanged<T>(this ObservableCollection<T> source) =>
             ((INotifyCollectionChanged)source).ObserveRemoveChanged<T>();

        /// <summary>Observe CollectionChanged:Remove.</summary>
        public static IObservable<T[]> ObserveRemoveChangedItems<T>(this ObservableCollection<T> source) =>
            ((INotifyCollectionChanged)source).ObserveRemoveChangedItems<T>();

        /// <summary>Observe CollectionChanged:Move and take single item.</summary>
        public static IObservable<OldNewPair<T>> ObserveMoveChanged<T>(this ObservableCollection<T> source) =>
            ((INotifyCollectionChanged)source).ObserveMoveChanged<T>();

        /// <summary>Observe CollectionChanged:Move.</summary>
        public static IObservable<OldNewPair<T[]>> ObserveMoveChangedItems<T>(this ObservableCollection<T> source) =>
            ((INotifyCollectionChanged)source).ObserveMoveChangedItems<T>();

        /// <summary>Observe CollectionChanged:Replace and take single item.</summary>
        public static IObservable<OldNewPair<T>> ObserveReplaceChanged<T>(this ObservableCollection<T> source) =>
            ((INotifyCollectionChanged)source).ObserveReplaceChanged<T>();

        /// <summary>Observe CollectionChanged:Replace.</summary>
        public static IObservable<OldNewPair<T[]>> ObserveReplaceChangedItems<T>(this ObservableCollection<T> source) =>
            ((INotifyCollectionChanged)source).ObserveReplaceChangedItems<T>();

        /// <summary>Observe CollectionChanged:Reset.</summary>
        public static IObservable<Unit> ObserveResetChanged<T>(this ObservableCollection<T> source) =>
            ((INotifyCollectionChanged)source).ObserveResetChanged<T>();

        /// <summary>
        /// Observe collection element's property.
        /// </summary>
        /// <typeparam name="TElement">Type of element</typeparam>
        /// <typeparam name="TProperty">Type of property</typeparam>
        /// <param name="source">Data source</param>
        /// <param name="propertySelector">Property selection expression</param>
        /// <param name="isPushCurrentValueAtFirst">Push current value on first subscribe</param>
        /// <returns>Property sequence</returns>
        public static IObservable<PropertyPack<TElement, TProperty>> ObserveElementProperty<TElement, TProperty>(this ObservableCollection<TElement> source, Expression<Func<TElement, TProperty>> propertySelector, bool isPushCurrentValueAtFirst = true)
            where TElement : class, INotifyPropertyChanged =>
            INotifyCollectionChangedExtensions.ObserveElementProperty(source, propertySelector, isPushCurrentValueAtFirst);

        /// <summary>
        /// Observe collection element's IObservable sequence.
        /// </summary>
        /// <typeparam name="TCollection">Collection type</typeparam>
        /// <typeparam name="TElement">Collection element type</typeparam>
        /// <typeparam name="TProperty">Type of observable property element</typeparam>
        /// <param name="source">Source collection</param>
        /// <param name="propertySelector">IObservable selection expression</param>
        /// <returns>IObservable sequence sequence</returns>
        public static IObservable<PropertyPack<TElement, TProperty>> ObserveElementObservableProperty<TElement, TProperty>(this ObservableCollection<TElement> source, Expression<Func<TElement, IObservable<TProperty>>> propertySelector)
            where TElement : class =>
            INotifyCollectionChangedExtensions.ObserveElementObservableProperty(source, propertySelector);

        /// <summary>
        ///  Observe collection element's PropertyChanged event.
        /// </summary>
        /// <typeparam name="TElement">Type of Element</typeparam>
        /// <param name="source">source collection</param>
        /// <returns>PropertyChanged event stream.</returns>
        public static IObservable<SenderEventArgsPair<TElement, PropertyChangedEventArgs>> ObserveElementPropertyChanged<TElement>(this ObservableCollection<TElement> source)
            where TElement : class, INotifyPropertyChanged =>
            INotifyCollectionChangedExtensions.ObserveElementPropertyChanged<ObservableCollection<TElement>, TElement>(source);
        #endregion


        #region ReadOnlyObservableCollection
        /// <summary>Observe CollectionChanged:Add and take single item.</summary>
        public static IObservable<T> ObserveAddChanged<T>(this ReadOnlyObservableCollection<T> source) =>
            ((INotifyCollectionChanged)source).ObserveAddChanged<T>();

        /// <summary>Observe CollectionChanged:Add.</summary>
        public static IObservable<T[]> ObserveAddChangedItems<T>(this ReadOnlyObservableCollection<T> source) =>
            ((INotifyCollectionChanged)source).ObserveAddChangedItems<T>();

        /// <summary>Observe CollectionChanged:Remove and take single item.</summary>
        public static IObservable<T> ObserveRemoveChanged<T>(this ReadOnlyObservableCollection<T> source) =>
            ((INotifyCollectionChanged)source).ObserveRemoveChanged<T>();

        /// <summary>Observe CollectionChanged:Remove.</summary>
        public static IObservable<T[]> ObserveRemoveChangedItems<T>(this ReadOnlyObservableCollection<T> source) =>
            ((INotifyCollectionChanged)source).ObserveRemoveChangedItems<T>();

        /// <summary>Observe CollectionChanged:Move and take single item.</summary>
        public static IObservable<OldNewPair<T>> ObserveMoveChanged<T>(this ReadOnlyObservableCollection<T> source) =>
            ((INotifyCollectionChanged)source).ObserveMoveChanged<T>();

        /// <summary>Observe CollectionChanged:Move.</summary>
        public static IObservable<OldNewPair<T[]>> ObserveMoveChangedItems<T>(this ReadOnlyObservableCollection<T> source) =>
            ((INotifyCollectionChanged)source).ObserveMoveChangedItems<T>();

        /// <summary>Observe CollectionChanged:Replace and take single item.</summary>
        public static IObservable<OldNewPair<T>> ObserveReplaceChanged<T>(this ReadOnlyObservableCollection<T> source) => 
            ((INotifyCollectionChanged)source).ObserveReplaceChanged<T>();

        /// <summary>Observe CollectionChanged:Replace.</summary>
        public static IObservable<OldNewPair<T[]>> ObserveReplaceChangedItems<T>(this ReadOnlyObservableCollection<T> source) =>
            ((INotifyCollectionChanged)source).ObserveReplaceChangedItems<T>();

        /// <summary>Observe CollectionChanged:Reset.</summary>
        public static IObservable<Unit> ObserveResetChanged<T>(this ReadOnlyObservableCollection<T> source) =>
            ((INotifyCollectionChanged)source).ObserveResetChanged<T>();

        /// <summary>
        /// Observe collection element's property.
        /// </summary>
        /// <typeparam name="TElement">Type of element</typeparam>
        /// <typeparam name="TProperty">Type of property</typeparam>
        /// <param name="source">Data source</param>
        /// <param name="propertySelector">Property selection expression</param>
        /// <param name="isPushCurrentValueAtFirst">Push current value on first subscribe.</param>
        /// <returns>Property sequence</returns>
        public static IObservable<PropertyPack<TElement, TProperty>> ObserveElementProperty<TElement, TProperty>(this ReadOnlyObservableCollection<TElement> source, Expression<Func<TElement, TProperty>> propertySelector, bool isPushCurrentValueAtFirst = true)
            where TElement : class, INotifyPropertyChanged =>
            INotifyCollectionChangedExtensions.ObserveElementProperty(source, propertySelector, isPushCurrentValueAtFirst);

        /// <summary>
        /// Observe collection element's IObservable sequence.
        /// </summary>
        /// <typeparam name="TCollection">Collection type</typeparam>
        /// <typeparam name="TElement">Collection element type</typeparam>
        /// <typeparam name="TProperty">Type of observable property element</typeparam>
        /// <param name="source">Source collection</param>
        /// <param name="propertySelector">IObservable selection expression</param>
        /// <returns>IObservable sequence sequence</returns>
        public static IObservable<PropertyPack<TElement, TProperty>> ObserveElementObservableProperty<TElement, TProperty>(this ReadOnlyObservableCollection<TElement> source, Expression<Func<TElement, IObservable<TProperty>>> propertySelector)
            where TElement : class =>
            INotifyCollectionChangedExtensions.ObserveElementObservableProperty(source, propertySelector);

        /// <summary>
        ///  Observe collection element's PropertyChanged event.
        /// </summary>
        /// <typeparam name="TElement">Type of Element</typeparam>
        /// <param name="source">source collection</param>
        /// <returns>PropertyChanged event stream.</returns>
        public static IObservable<SenderEventArgsPair<TElement, PropertyChangedEventArgs>> ObserveElementPropertyChanged<TElement>(this ReadOnlyObservableCollection<TElement> source)
            where TElement : class, INotifyPropertyChanged =>
            INotifyCollectionChangedExtensions.ObserveElementPropertyChanged<ReadOnlyObservableCollection<TElement>, TElement>(source);
        #endregion
    }
}