using System;
using System.ComponentModel;

namespace Reactive.Bindings
{
    /// <summary>
    /// for EventToReactive and Serialization
    /// </summary>
    public interface IReactiveProperty : IReadOnlyReactiveProperty, IHasErrors, INotifyPropertyChanged
    {
        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>The value.</value>
        new object Value { get; set; }
    }

    /// <summary>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <seealso cref="Reactive.Bindings.IHasErrors"/>
    public interface IReactiveProperty<T> : IReactiveProperty, IReadOnlyReactiveProperty<T>, IObservable<T>, IDisposable, INotifyDataErrorInfo
    {
        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>The value.</value>
        new T Value { get; set; }
    }
}
