using System;

#if NETFX_CORE
using Windows.UI.Xaml;
#endif

namespace Reactive.Bindings.Interactivity
{
    /// <summary>
    /// </summary>
    public interface IEventToReactiveConverter
    {
        /// <summary>
        /// Gets or sets the associate object.
        /// </summary>
        /// <value>The associate object.</value>
        object AssociateObject { get; set; }

        /// <summary>
        /// Converts the specified source.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns></returns>
        IObservable<object> Convert(IObservable<object> source);
    }
}
