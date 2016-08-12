namespace Reactive.Bindings.Extensions
{
    /// <summary>
    /// Represents event sender and argument pair.
    /// </summary>
    /// <typeparam name="TElement">Type of event sender</typeparam>
    /// <typeparam name="TEventArgs">Type of event arguments</typeparam>
    public class SenderEventArgsPair<TSender, TEventArgs>
    {
        #region Properies
        /// <summary>
        /// Gets event sender.
        /// </summary>
        public TSender Sender { get; }


        /// <summary>
        /// Gets event argument.
        /// </summary>
        public TEventArgs EventArgs { get; }
        #endregion


        #region Constructors
        /// <summary>
        /// Create instance.
        /// </summary>
        /// <param name="sender">sender value</param>
        /// <param name="eventArgs">event arguments</param>
        internal SenderEventArgsPair(TSender sender, TEventArgs eventArgs)
        {
            this.Sender = sender;
            this.EventArgs = eventArgs;
        }
        #endregion
    }



    /// <summary>
    /// Provides SenderEventArgsPair static members.
    /// </summary>
    internal static class SenderEventArgsPair
    {
        /// <summary>
        /// Create instance.
        /// </summary>
        /// <typeparam name="TElement">Type of event sender</typeparam>
        /// <typeparam name="TEventArgs">Type of event arguments</typeparam>
        /// <param name="sender">sender value</param>
        /// <param name="eventArgs">event arguments</param>
        /// <returns>Created instance.</returns>
        public static SenderEventArgsPair<TSender, TEventArgs> Create<TSender, TEventArgs>(TSender sender, TEventArgs eventArgs) =>
            new SenderEventArgsPair<TSender, TEventArgs>(sender, eventArgs);
    }
}