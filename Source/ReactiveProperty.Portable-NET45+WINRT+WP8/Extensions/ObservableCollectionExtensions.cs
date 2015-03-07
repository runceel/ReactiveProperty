using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;

namespace Reactive.Bindings.Extensions
{
    public static class ObservableCollectionExtensions
    {
        /// <summary>Observe CollectionChanged:Add and take single item.</summary>
        public static IObservable<T> ObserveAddChanged<T>(this ObservableCollection<T> source)
        {
            return ((INotifyCollectionChanged)source).ObserveAddChanged<T>();
        }

        /// <summary>Observe CollectionChanged:Add.</summary>
        public static IObservable<T[]> ObserveAddChangedItems<T>(this ObservableCollection<T> source)
        {
            return ((INotifyCollectionChanged)source).ObserveAddChangedItems<T>();
        }

        /// <summary>Observe CollectionChanged:Remove and take single item.</summary>
        public static IObservable<T> ObserveRemoveChanged<T>(this ObservableCollection<T> source)
        {
            return ((INotifyCollectionChanged)source).ObserveRemoveChanged<T>();
        }

        /// <summary>Observe CollectionChanged:Remove.</summary>
        public static IObservable<T[]> ObserveRemoveChangedItems<T>(this ObservableCollection<T> source)
        {
            return ((INotifyCollectionChanged)source).ObserveRemoveChangedItems<T>();
        }

        /// <summary>Observe CollectionChanged:Move and take single item.</summary>
        public static IObservable<OldNewPair<T>> ObserveMoveChanged<T>(this ObservableCollection<T> source)
        {
            return ((INotifyCollectionChanged)source).ObserveMoveChanged<T>();
        }

        /// <summary>Observe CollectionChanged:Move.</summary>
        public static IObservable<OldNewPair<T[]>> ObserveMoveChangedItems<T>(this ObservableCollection<T> source)
        {
            return ((INotifyCollectionChanged)source).ObserveMoveChangedItems<T>();
        }

        /// <summary>Observe CollectionChanged:Replace and take single item.</summary>
        public static IObservable<OldNewPair<T>> ObserveReplaceChanged<T>(this ObservableCollection<T> source)
        {
            return ((INotifyCollectionChanged)source).ObserveReplaceChanged<T>();
        }

        /// <summary>Observe CollectionChanged:Replace.</summary>
        public static IObservable<OldNewPair<T[]>> ObserveReplaceChangedItems<T>(this ObservableCollection<T> source)
        {
            return ((INotifyCollectionChanged)source).ObserveReplaceChangedItems<T>();
        }

        /// <summary>Observe CollectionChanged:Reset.</summary>
        public static IObservable<Unit> ObserveResetChanged<T>(this ObservableCollection<T> source)
        {
            return ((INotifyCollectionChanged)source).ObserveResetChanged<T>();
        }

        // readonlycollection

        /// <summary>Observe CollectionChanged:Add and take single item.</summary>
        public static IObservable<T> ObserveAddChanged<T>(this ReadOnlyObservableCollection<T> source)
        {
            return ((INotifyCollectionChanged)source).ObserveAddChanged<T>();
        }

        /// <summary>Observe CollectionChanged:Add.</summary>
        public static IObservable<T[]> ObserveAddChangedItems<T>(this ReadOnlyObservableCollection<T> source)
        {
            return ((INotifyCollectionChanged)source).ObserveAddChangedItems<T>();
        }

        /// <summary>Observe CollectionChanged:Remove and take single item.</summary>
        public static IObservable<T> ObserveRemoveChanged<T>(this ReadOnlyObservableCollection<T> source)
        {
            return ((INotifyCollectionChanged)source).ObserveRemoveChanged<T>();
        }

        /// <summary>Observe CollectionChanged:Remove.</summary>
        public static IObservable<T[]> ObserveRemoveChangedItems<T>(this ReadOnlyObservableCollection<T> source)
        {
            return ((INotifyCollectionChanged)source).ObserveRemoveChangedItems<T>();
        }

        /// <summary>Observe CollectionChanged:Move and take single item.</summary>
        public static IObservable<OldNewPair<T>> ObserveMoveChanged<T>(this ReadOnlyObservableCollection<T> source)
        {
            return ((INotifyCollectionChanged)source).ObserveMoveChanged<T>();
        }

        /// <summary>Observe CollectionChanged:Move.</summary>
        public static IObservable<OldNewPair<T[]>> ObserveMoveChangedItems<T>(this ReadOnlyObservableCollection<T> source)
        {
            return ((INotifyCollectionChanged)source).ObserveMoveChangedItems<T>();
        }

        /// <summary>Observe CollectionChanged:Replace and take single item.</summary>
        public static IObservable<OldNewPair<T>> ObserveReplaceChanged<T>(this ReadOnlyObservableCollection<T> source)
        {
            return ((INotifyCollectionChanged)source).ObserveReplaceChanged<T>();
        }

        /// <summary>Observe CollectionChanged:Replace.</summary>
        public static IObservable<OldNewPair<T[]>> ObserveReplaceChangedItems<T>(this ReadOnlyObservableCollection<T> source)
        {
            return ((INotifyCollectionChanged)source).ObserveReplaceChangedItems<T>();
        }

        /// <summary>Observe CollectionChanged:Reset.</summary>
        public static IObservable<Unit> ObserveResetChanged<T>(this ReadOnlyObservableCollection<T> source)
        {
            return ((INotifyCollectionChanged)source).ObserveResetChanged<T>();
        }

    }
}