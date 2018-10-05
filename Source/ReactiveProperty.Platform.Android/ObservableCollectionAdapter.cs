using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Android.Views;

namespace Reactive.Bindings
{
    /// <summary>
    /// INotifyCollectionChanged ListAdapter
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ObservableCollectionAdapter<T> : ListAdapter<T>, IDisposable
    {
        private INotifyCollectionChanged List { get; }

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="list">source list</param>
        /// <param name="createRowView">create row view</param>
        /// <param name="setRowData">set row data</param>
        /// <param name="getId">get id</param>
        public ObservableCollectionAdapter(INotifyCollectionChanged list, Func<int, T, View> createRowView, Action<int, T, View> setRowData, Func<int, T, long> getId = null)
            : base(list as IList<T>, createRowView, setRowData, getId)
        {
            if (!(list is IList<T>)) { throw new ArgumentException(nameof(list)); }
            List = list;
            List.CollectionChanged += CollectionChanged;
        }

        /// <summary>
        /// To be added.
        /// </summary>
        /// <param name="disposing">To be added.</param>
        /// <remarks>To be added.</remarks>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing) {
                List.CollectionChanged -= CollectionChanged;
            }
        }

        private void CollectionChanged(object sender, NotifyCollectionChangedEventArgs e) => NotifyDataSetChanged();
    }

    /// <summary>
    /// Observable Collection Extensions
    /// </summary>
    public static class ObservableCollectionExtensions
    {
        /// <summary>
        /// Create ObservableCollectionAdapter from ObservableCollection
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="self"></param>
        /// <param name="createRowView"></param>
        /// <param name="setRowData"></param>
        /// <param name="getId"></param>
        /// <returns></returns>
        public static ObservableCollectionAdapter<T> ToAdapter<T>(this ObservableCollection<T> self, Func<int, T, View> createRowView, Action<int, T, View> setRowData, Func<int, T, long> getId = null) =>
            new ObservableCollectionAdapter<T>(self, createRowView, setRowData, getId);
    }

    /// <summary>
    /// ReadOnly Observable Collection Extensions
    /// </summary>
    public static class ReadOnlyObservableCollectionExtensions
    {
        /// <summary>
        /// Create ObservableCollectionAdapter from ReadOnlyObservableCollection
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="self"></param>
        /// <param name="createRowView"></param>
        /// <param name="setRowData"></param>
        /// <param name="getId"></param>
        /// <returns></returns>
        public static ObservableCollectionAdapter<T> ToAdapter<T>(this ReadOnlyObservableCollection<T> self, Func<int, T, View> createRowView, Action<int, T, View> setRowData, Func<int, T, long> getId = null) =>
            new ObservableCollectionAdapter<T>(self, createRowView, setRowData, getId);
    }
}
