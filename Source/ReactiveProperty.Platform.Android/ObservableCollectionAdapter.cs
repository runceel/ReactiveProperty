using Android.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

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
            this.List = list;
            this.List.CollectionChanged += this.CollectionChanged;
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                this.List.CollectionChanged -= this.CollectionChanged;
            }
        }

        private void CollectionChanged(object sender, NotifyCollectionChangedEventArgs e) => this.NotifyDataSetChanged();

    }

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