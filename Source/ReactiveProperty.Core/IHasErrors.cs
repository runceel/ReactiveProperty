using System;
using System.Collections;

namespace Reactive.Bindings
{
    /// <summary>
    /// </summary>
    public interface IHasErrors
    {
        /// <summary>
        /// Gets the observe error changed.
        /// </summary>
        /// <value>The observe error changed.</value>
        IObservable<IEnumerable> ObserveErrorChanged { get; }

        /// <summary>
        /// Gets the observe has errors.
        /// </summary>
        /// <value>The observe has errors.</value>
        IObservable<bool> ObserveHasErrors { get; }
    }
}
