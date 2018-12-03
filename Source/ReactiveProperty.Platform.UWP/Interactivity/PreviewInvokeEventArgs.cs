using System;

namespace Reactive.Bindings.Interactivity
{
    /// <summary>
    /// Preview Invoke EventArgs
    /// </summary>
    /// <seealso cref="System.EventArgs"/>
    public class PreviewInvokeEventArgs : EventArgs
    {
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="PreviewInvokeEventArgs"/> is cancelling.
        /// </summary>
        /// <value><c>true</c> if cancelling; otherwise, <c>false</c>.</value>
        public bool Cancelling { get; set; }
    }
}
