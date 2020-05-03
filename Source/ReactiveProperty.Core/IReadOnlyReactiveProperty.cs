using System;
using System.ComponentModel;

namespace Reactive.Bindings
{
    /// <summary>
    /// </summary>
    public interface IReadOnlyReactiveProperty
    {
        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <value>The value.</value>
        object Value { get; }
    }

    /// <summary>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IReadOnlyReactiveProperty<out T> : IReadOnlyReactiveProperty, IObservable<T>, INotifyPropertyChanged, IDisposable
    {
        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <value>The value.</value>
        new T Value { get; }
    }
}
