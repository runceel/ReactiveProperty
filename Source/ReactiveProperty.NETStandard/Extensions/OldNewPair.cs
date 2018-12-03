namespace Reactive.Bindings.Extensions
{
    /// <summary>
    /// Value pair of OldItem and NewItem.
    /// </summary>
    public class OldNewPair<T>
    {
        /// <summary>
        /// Gets the old item.
        /// </summary>
        /// <value>The old item.</value>
        public T OldItem { get; }

        /// <summary>
        /// Gets the new item.
        /// </summary>
        /// <value>The new item.</value>
        public T NewItem { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="OldNewPair{T}"/> class.
        /// </summary>
        /// <param name="oldItem">The old item.</param>
        /// <param name="newItem">The new item.</param>
        public OldNewPair(T oldItem, T newItem)
        {
            OldItem = oldItem;
            NewItem = newItem;
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String"/> that represents this instance.</returns>
        public override string ToString()
        {
            return "{ Old = " + OldItem + ", New = " + NewItem + " }";
        }
    }
}
