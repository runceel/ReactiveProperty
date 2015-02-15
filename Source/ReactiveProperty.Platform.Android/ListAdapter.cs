using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;

namespace Reactive.Bindings
{
    /// <summary>
    /// Generic IList Adapter
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ListAdapter<T> : BaseAdapter<T>
    {
        private IList<T> list;

        private Func<int, T, View> createRowView;

        private Action<int, T, View> setRowData;

        private Func<int, T, long> getId;

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="list">source list</param>
        /// <param name="createRowView">create view</param>
        /// <param name="setRowData">set row data</param>
        /// <param name="getId">get id</param>
        public ListAdapter(IList<T> list, Func<int, T, View> createRowView, Action<int, T, View> setRowData, Func<int, T, long> getId = null)
        {
            if (list == null) { throw new ArgumentNullException("list"); }
            if (createRowView == null) { throw new ArgumentNullException("createRowView"); }
            if (setRowData == null) { throw new ArgumentNullException("setRowData"); }

            this.list = list;
            this.createRowView = createRowView;
            this.setRowData = setRowData;
            this.getId = getId ?? ((index, _) => index);
        }

        public override T this[int position]
        {
            get { return this.list[position]; }
        }

        public override int Count
        {
            get { return this.list.Count; }
        }

        public override long GetItemId(int position)
        {
            return this.getId(position, this[position]);
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            if (convertView == null)
            {
                convertView = createRowView(position, this[position]);
            }

            setRowData(position, this[position], convertView);
            return convertView;
        }
    }

    public static class ListExtensions
    {
        /// <summary>
        /// Convert IList to ListAdapter
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="self">IList</param>
        /// <param name="createRowView">create view</param>
        /// <param name="setRowData">fill row data</param>
        /// <param name="getId">get id</param>
        /// <returns>ListAdapter</returns>
        public static ListAdapter<T> ToAdapter<T>(this IList<T> self, Func<int, T, View> createRowView, Action<int, T, View> setRowData, Func<int, T, long> getId = null)
        {
            return new ListAdapter<T>(self, createRowView, setRowData, getId);
        }
    }
}
