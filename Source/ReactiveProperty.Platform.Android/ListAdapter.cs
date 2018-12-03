using System;
using System.Collections.Generic;
using Android.Views;
using Android.Widget;

namespace Reactive.Bindings
{
    /// <summary>
    /// Generic IList Adapter
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ListAdapter<T> : BaseAdapter<T>
    {
        private IList<T> List { get; }

        private Func<int, T, View> CreateRowView { get; }

        private Action<int, T, View> SetRowData { get; }

        private Func<int, T, long> GetId { get; }

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="list">source list</param>
        /// <param name="createRowView">create view</param>
        /// <param name="setRowData">set row data</param>
        /// <param name="getId">get id</param>
        public ListAdapter(IList<T> list, Func<int, T, View> createRowView, Action<int, T, View> setRowData, Func<int, T, long> getId = null)
        {
            List = list ?? throw new ArgumentNullException(nameof(list));
            CreateRowView = createRowView ?? throw new ArgumentNullException(nameof(createRowView));
            SetRowData = setRowData ?? throw new ArgumentNullException(nameof(setRowData));
            GetId = getId ?? ((index, _) => index);
        }

        /// <summary>
        /// Gets the item with the specified position.
        /// </summary>
        /// <value>The type.</value>
        /// <param name="position">The position.</param>
        /// <returns></returns>
        public override T this[int position] => List[position];

        /// <summary>
        /// To be added.
        /// </summary>
        /// <value>To be added.</value>
        /// <remarks>To be added.</remarks>
        public override int Count => List.Count;

        /// <summary>
        /// To be added.
        /// </summary>
        /// <param name="position">To be added.</param>
        /// <returns>To be added.</returns>
        /// <remarks>To be added.</remarks>
        public override long GetItemId(int position) => GetId(position, this[position]);

        /// <summary>
        /// To be added.
        /// </summary>
        /// <param name="position">To be added.</param>
        /// <param name="convertView">To be added.</param>
        /// <param name="parent">To be added.</param>
        /// <returns>To be added.</returns>
        /// <remarks>To be added.</remarks>
        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            if (convertView == null) {
                convertView = CreateRowView(position, this[position]);
            }

            SetRowData(position, this[position], convertView);
            return convertView;
        }
    }

    /// <summary>
    /// List Extensions
    /// </summary>
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
        public static ListAdapter<T> ToAdapter<T>(this IList<T> self, Func<int, T, View> createRowView, Action<int, T, View> setRowData, Func<int, T, long> getId = null) =>
            new ListAdapter<T>(self, createRowView, setRowData, getId);
    }
}
